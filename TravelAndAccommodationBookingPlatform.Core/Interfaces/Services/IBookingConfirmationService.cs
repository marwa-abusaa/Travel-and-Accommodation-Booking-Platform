using TravelAndAccommodationBookingPlatform.Core.Entities;

namespace TravelAndAccommodationBookingPlatform.Core.Interfaces.Services;

public interface IBookingConfirmationService
{
    Task SendBookingConfirmationAsync(Booking booking);
}
