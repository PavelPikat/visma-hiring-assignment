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

            return new PdfOrPngResult
            {
                Stream = fileStream,
                ContentType = response.Content.Headers.ContentType?.ToString()
            };
        }
        catch (Exception e)
        {
            if (e is CorruptedResponseException)
            {
                _logger.LogWarning("Received invalid PDF from {Client}", ServiceConfigOptions.PdfClientName);
            }
            else
            {
                _logger.LogError(e, "Failed to load dummy pdf/png by id '{Id}' using {Client} client", id,
                    ServiceConfigOptions.PdfClientName);
            }

            throw;
        }
    }
}
