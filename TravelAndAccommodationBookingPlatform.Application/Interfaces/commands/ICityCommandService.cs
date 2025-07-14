using TravelAndAccommodationBookingPlatform.Application.Dtos.Cities;

namespace TravelAndAccommodationBookingPlatform.Application.Interfaces.Commands;

public interface ICityCommandService
{
    Task<AdminCityResponseDto> AddCityAsync(CreateCityDto createCityDto);    
    Task DeleteCityByIdAsync(int cityId);
    Task UpdateCityAsync(UpdateCityDto updateCityDto);
}