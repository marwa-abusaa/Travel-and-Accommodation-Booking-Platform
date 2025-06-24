using TravelAndAccommodationBookingPlatform.Core.Entities;

namespace TravelAndAccommodationBookingPlatform.Core.Interfaces.Services
{
    public interface IPdfGeneratorService
    {
        public Task SavePaymentConfirmationAsPdf(Invoice invoice, string filePath);
    }
}