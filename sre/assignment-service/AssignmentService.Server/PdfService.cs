using AssignmentService.Server.Exceptions;

namespace AssignmentService.Server;

public class PdfService : IPdfService
{
    private readonly ILogger<PdfService> _logger;
    private readonly IHttpClientFactory _httpClientFactory;

    public PdfService(ILogger<PdfService> logger, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<PdfOrPngResult> GetPdfOrPngStream(int id)
    {
        try
        {
            var dummyPdfClient = _httpClientFactory.CreateClient(ServiceConfigOptions.PdfClientName);
            var response = await dummyPdfClient.GetAsync("/");
            var contentType = response.Content.Headers.ContentType?.ToString();

            if (string.IsNullOrEmpty(contentType))
            {
                throw new InvalidContentTypeException();
            }

            _logger.LogInformation("Received {ContentType} from {Client}", contentType,
                ServiceConfigOptions.PdfClientName);
            var fileStream = await response.Content.ReadAsStreamAsync();

            switch (contentType)
            {
                case "application/png":
                    break;
                case "application/pdf":
                    var isValidPdf = PdfValidator.ValidatePdf(fileStream);
                    if (!isValidPdf)
                    {
                        throw new CorruptedResponseException();
                    }

                    break;
            }

            PrometheusMetrics.ServedContentTypes.WithLabels(contentType).Inc();

            return new PdfOrPngResult
            {
                Stream = fileStream,
                ContentType = response.Content.Headers.ContentType?.ToString()
            };
        }
        catch (Exception e)
        {
            switch (e)
            {
                case CorruptedResponseException:
                    PrometheusMetrics.CorruptedPdfs.Inc();
                    _logger.LogWarning("Received invalid PDF from {Client}", ServiceConfigOptions.PdfClientName);
                    break;
                case InvalidContentTypeException:
                    _logger.LogError(e, "Received empty content type header from {Client}", ServiceConfigOptions.PdfClientName);
                    break;
                default:
                    _logger.LogError(e, "Failed to load dummy pdf/png by id '{Id}' using {Client} client", id,
                        ServiceConfigOptions.PdfClientName);
                    break;
            }

            throw;
        }
    }
}
