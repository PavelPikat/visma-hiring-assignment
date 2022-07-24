namespace AssignmentService.Server;

public interface IPdfService
{
    Task<PdfOrPngResult> GetPdfOrPngStream(int id);
}
