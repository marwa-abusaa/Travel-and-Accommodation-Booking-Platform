using TravelAndAccommodationBookingPlatform.Application.Dtos.Bookings;

namespace TravelAndAccommodationBookingPlatform.Application.Interfaces.Commands;

public interface IBookingCommandService
{
    Task<BookingResponseDto> AddBookingAsync(CreateBookingDto createBookingDto);
    Task DeleteBookingByIdAsync(int bookingId);
}
