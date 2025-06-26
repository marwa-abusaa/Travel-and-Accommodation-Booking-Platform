using TravelAndAccommodationBookingPlatform.Core.Entities;

namespace TravelAndAccommodationBookingPlatform.Core.Interfaces.Repositories;

public interface IBookingRepository
{
    Task<Booking> AddBookingAsync(Booking booking);
    Task<Booking?> GetBookingByIdAsync(int bookingId);
    Task DeleteBookingByIdAsync(int bookingId);
    Task<IEnumerable<Booking>> GetBookingsByUserIdAsync(int userId);
}
