namespace AssignmentService.Server;

public class ServiceConfigOptions
{
    public const string ServiceConfig = "ServiceConfig";
    public const string PdfClientName = "DummyPdfOrPng";
    public string DummyPdfOrPngServiceRoot { get; set; }
    public int TransientHttpErrorRetryCount { get; set; }
    public int CorruptedPdfErrorRetryCount { get; set; }
}
