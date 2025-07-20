using TravelAndAccommodationBookingPlatform.Application.Dtos.Bookings;

namespace TravelAndAccommodationBookingPlatform.Application.Interfaces.Queries;

public interface IBookingQueryService
{
    Task<BookingResponseDto?> GetBookingByIdAsync(int bookingId);
    Task<IEnumerable<BookingResponseDto>> GetBookingsByUserIdAsync(int userId);
}
