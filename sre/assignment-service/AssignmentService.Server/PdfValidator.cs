using iText.Kernel.Pdf;

namespace AssignmentService.Server;

public static class PdfValidator
{
    public static bool ValidatePdf(Stream pdfStream)
    {
        try
        {
            using var reader = new PdfReader(pdfStream);
            using var pdfDoc = new PdfDocument(reader);
            pdfStream.Position = 0;

            return true;
        }
        catch
        {
            // ignored
        }

        return false;
    }
}
