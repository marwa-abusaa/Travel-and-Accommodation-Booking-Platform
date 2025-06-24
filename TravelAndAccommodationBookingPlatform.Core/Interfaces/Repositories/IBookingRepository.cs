using TravelAndAccommodationBookingPlatform.Core.Entities;

namespace TravelAndAccommodationBookingPlatform.Core.Interfaces.Repositories;

public interface IBookingRepository
{
    public Task<Booking> AddBookingAsync(Booking booking);
    public Task<Booking?> GetBookingByIdAsync(int bookingId);
    public Task DeleteBookingAsync(int bookingId);
    public Task<IEnumerable<Booking>> GetAllBookingsByUserAsync(int userId);
    public void PrintPaymentConfirmation(Invoice invoice);
}
