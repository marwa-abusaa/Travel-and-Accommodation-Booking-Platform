using TravelAndAccommodationBookingPlatform.Core.Models;

namespace TravelAndAccommodationBookingPlatform.Core.Interfaces.Services;

public interface IEmailService
{
    public Task SendPaymentConfirmationEmailAsync(EmailRequest emailRequest);
}
