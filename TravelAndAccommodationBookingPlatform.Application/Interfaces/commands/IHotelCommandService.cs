using TravelAndAccommodationBookingPlatform.Application.Dtos.Hotels;

namespace TravelAndAccommodationBookingPlatform.Application.Interfaces.Commands;

public interface IHotelCommandService
{
    Task<HotelResponseDto> AddHotelAsync(CreateHotelDto createHotelDto);
    Task DeleteHotelByIdAsync(int hotelId);
    Task UpdateHotelAsync(UpdateHotelDto updateHotelDto);
}