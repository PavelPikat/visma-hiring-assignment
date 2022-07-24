using System.Net;
using AssignmentService.Server;
using AssignmentService.Server.Exceptions;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;
using Prometheus;

var config = new ServiceConfigOptions();
var timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(10);

var builder = WebApplication.CreateBuilder(args);

// App configuration
builder.Configuration
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables($"{ServiceConfigOptions.AppPrefix.ToUpper()}_");
builder.Logging.AddConsole();

using var loggerFactory = LoggerFactory.Create(loggingBuilder => loggingBuilder.AddConsole());
var logger = loggerFactory.CreateLogger<Program>();

builder.Services
    .AddOptions<ServiceConfigOptions>()
    .Bind(builder.Configuration.GetSection(ServiceConfigOptions.ServiceConfig));

builder.Services.AddTransient<IPdfService, PdfService>();

builder.Configuration.Bind(ServiceConfigOptions.ServiceConfig, config);

var retryPolicy = HttpPolicyExtensions
    .HandleTransientHttpError()
    .Or<TimeoutRejectedException>()
    .RetryAsync(config.TransientHttpErrorRetryCount,
        onRetry: (result, attempt) =>
        {
            PrometheusMetrics.FailedDownstreamRequests.WithLabels("dummy_pdf_or_png").Inc();
            logger.LogWarning("Transient HTTP error: {Error}. Attempt {Attempt}/{MaxAttempts}...",
                result.Exception.Message, attempt, config.TransientHttpErrorRetryCount);
        });

builder
    .Services
    .AddHttpClient(ServiceConfigOptions.PdfClientName, c => c.BaseAddress = new Uri(config.DummyPdfOrPngServiceRoot))
    .AddPolicyHandler(retryPolicy)
    .AddPolicyHandler(timeoutPolicy);

var hcBuilder = builder.Services.AddHealthChecks();
hcBuilder.AddCheck("ready", _ => HealthCheckResult.Healthy());

// App
var app = builder.Build();

app.UseHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = r => r.Tags.Contains("ready")
});

app.UseHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = _ => true
});

app.UseHealthChecksPrometheusExporter("/health/metrics",
    options => options.ResultStatusCodes[HealthStatus.Unhealthy] = (int)HttpStatusCode.OK);

app.MapGet("/{id:int}", async (int id, IPdfService pdfService) =>
{
    try
    {
        var policyResult = await Policy.Handle<CorruptedResponseException>()
            .RetryAsync(config.CorruptedPdfErrorRetryCount,
                onRetry: (_, attempt) =>
                {
                    logger.LogWarning("Not returning corrupted PDF to the client, retrying {Attempt}/{MaxAttempts}...", attempt,
                        config.CorruptedPdfErrorRetryCount);
                })
            .ExecuteAndCaptureAsync(async () => await pdfService.GetPdfOrPngStream(id));

        return policyResult.Outcome == OutcomeType.Successful
            ? Results.Stream(policyResult.Result.Stream, policyResult.Result.ContentType)
            : Results.Problem();
    }
    catch
    {
        return Results.Problem("Oops, something went wrong. Please try again later");
    }
});

app.UseMetricServer();
app.UseHttpMetrics();
app.Run();
