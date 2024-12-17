using DinkToPdf;
using DinkToPdf.Contracts;

namespace EduTech.Services;

public class PdfConverter
{
    private readonly IConverter _converter;

    public PdfConverter(IConverter converter)
    {
        _converter = converter;
    }

    public byte[] ConvertToPdf(string htmlContent)
    {
        var doc = new HtmlToPdfDocument()
        {
            GlobalSettings = {
                PaperSize = PaperKind.A4,
                Orientation = Orientation.Portrait
            },
            Objects = {
                new ObjectSettings() {
                    HtmlContent = htmlContent,
                    WebSettings = { DefaultEncoding = "utf-8" }
                }
            }
        };

        return _converter.Convert(doc);
    }
}