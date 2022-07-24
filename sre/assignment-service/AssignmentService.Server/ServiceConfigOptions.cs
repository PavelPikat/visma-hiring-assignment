namespace AssignmentService.Server;

public class ServiceConfigOptions
{
    public const string ServiceConfig = "ServiceConfig";
    public string DummyPdfOrPngServiceRoot { get; set; }
    public int TransientHttpErrorRetryCount { get; set; }
}
