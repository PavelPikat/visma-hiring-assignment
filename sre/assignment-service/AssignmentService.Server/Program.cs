using AssignmentService.Server;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;

var config = new ServiceConfigOptions();
var timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(10);

var builder = WebApplication.CreateBuilder(args);

// App configuration
builder.Configuration
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables();
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
            logger.LogWarning("Transient HTTP error: {Error}. Attempt {Attempt}/{MaxAttempts}...",
                result.Exception.Message, attempt, config.TransientHttpErrorRetryCount);
        });

builder
    .Services
    .AddHttpClient(ServiceConfigOptions.PdfClientName, c => c.BaseAddress = new Uri(config.DummyPdfOrPngServiceRoot))
    .AddPolicyHandler(retryPolicy)
    .AddPolicyHandler(timeoutPolicy);

var app = builder.Build();
app.MapGet("/{id}", async (int id, IPdfService pdfService) =>
{
    try
    {
        var policyResult = await Policy.Handle<CorruptedResponseException>()
            .RetryAsync(config.CorruptedPdfErrorRetryCount,
                onRetry: (_, attempt) =>
                {
                    logger.LogWarning("Received corrupted PDF. Attempt {Attempt}/{MaxAttempts}...", attempt,
                        config.CorruptedPdfErrorRetryCount);
                })
            .ExecuteAndCaptureAsync(async () => await pdfService.GetPdfOrPngStream(id));
        
        return Results.Stream(policyResult.Result.Stream, policyResult.Result.ContentType);
    }
    catch
    {
        return Results.Problem("Oops, something went wrong. Please try again later");
    }
});

app.Run();
