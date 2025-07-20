namespace TravelAndAccommodationBookingPlatform.Core.Interfaces.Services;

public interface IPdfGeneratorService
{
    byte[] GeneratePdfFromHtml(string htmlContent);
}