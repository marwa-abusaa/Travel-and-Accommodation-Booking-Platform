using DinkToPdf;
using DinkToPdf.Contracts;
using TravelAndAccommodationBookingPlatform.Core.Interfaces.Services;

namespace TravelAndAccommodationBookingPlatform.Infrastructure.Services;

public class PdfGeneratorService : IPdfGeneratorService
{
    private readonly IConverter _converter;

    public PdfGeneratorService(IConverter converter)
    {
        _converter = converter;
    }
    public byte[] GeneratePdfFromHtml(string htmlContent)
    {
        var doc = new HtmlToPdfDocument()
        {
            GlobalSettings = {
                PaperSize = PaperKind.A4,
                Orientation = Orientation.Portrait
            },
            Objects = {
                new ObjectSettings {
                    HtmlContent = htmlContent
                }
            }
        };

        return _converter.Convert(doc);
    }
}
