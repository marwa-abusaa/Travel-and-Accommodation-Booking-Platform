using NReco.PdfGenerator;
using TravelAndAccommodationBookingPlatform.Core.Interfaces.Services;

namespace TravelAndAccommodationBookingPlatform.Infrastructure.Services;

public class PdfGeneratorService : IPdfGeneratorService
{
    public byte[] GeneratePdfFromHtml(string htmlContent)
    {
        var converter = new HtmlToPdfConverter();
        return converter.GeneratePdf(htmlContent);
    }
}
