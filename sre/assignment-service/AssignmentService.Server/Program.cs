using AssignmentService.Server;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;

const string dummyPdfClientName = "DummyPdfOrPng";
var config = new ServiceConfigOptions();
var timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(10);

var builder = WebApplication.CreateBuilder(args);

// App configuration
builder.Configuration
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables();
builder.Logging.AddConsole();

var logger = builder.Logging.Services.BuildServiceProvider().GetService<ILogger<Program>>();

builder.Services
    .AddOptions<ServiceConfigOptions>()
    .Bind(builder.Configuration.GetSection(ServiceConfigOptions.ServiceConfig));

builder.Configuration.Bind(ServiceConfigOptions.ServiceConfig, config);

var retryPolicy = HttpPolicyExtensions
    .HandleTransientHttpError()
    .Or<TimeoutRejectedException>()
    .Or<CorruptedResponseException>()
    .RetryAsync(config.TransientHttpErrorRetryCount, onRetry: (result, i, context) =>
    {
        logger.LogWarning("Transient HTTP error: {Error}. Attempt {attempt}/{maxAttempts}...", result.Exception.Message, i, config.TransientHttpErrorRetryCount);
    });

builder
    .Services
    .AddHttpClient(dummyPdfClientName, c => c.BaseAddress = new Uri(config.DummyPdfOrPngServiceRoot))
    .AddPolicyHandler(retryPolicy)
    .AddPolicyHandler(timeoutPolicy);

var app = builder.Build();
app.MapGet("/{id}", async (int id, IHttpClientFactory httpClientFactory, ILoggerFactory loggerFactory) =>
{
    var logger = loggerFactory.CreateLogger(dummyPdfClientName);
    try
    {
        var dummyPdfClient = httpClientFactory.CreateClient(dummyPdfClientName);
        var pdfResponse = await dummyPdfClient.GetAsync("/");
        var contentType = pdfResponse.Content.Headers.ContentType?.ToString();

        logger.LogInformation("Received {ContentType} from {Client}", contentType, dummyPdfClientName);

        return Results.Stream(await pdfResponse.Content.ReadAsStreamAsync(),
            pdfResponse.Content.Headers.ContentType?.ToString());
    }
    catch (Exception e)
    {
        logger.LogError(e, "Failed to load dummy pdf/png by id '{Id}' using {Client} client", id, dummyPdfClientName);
        return Results.Problem("Oops, something went wrong. Please try again later");
    }
});

app.Run();
