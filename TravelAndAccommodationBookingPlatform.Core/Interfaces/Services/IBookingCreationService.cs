using TravelAndAccommodationBookingPlatform.Core.Entities;

namespace TravelAndAccommodationBookingPlatform.Core.Interfaces.Services;

public interface IBookingCreationService
{
    Task<Booking> CreateBookingAsync(Booking booking);
}
