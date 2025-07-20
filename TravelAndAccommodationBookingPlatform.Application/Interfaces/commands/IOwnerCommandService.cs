using TravelAndAccommodationBookingPlatform.Application.Dtos.Owners;

namespace TravelAndAccommodationBookingPlatform.Application.Interfaces.Commands;

public interface IOwnerCommandService
{
    Task<OwnerResponseDto> AddOwnerAsync(CreateOwnerDto createOwnerDto);
    Task UpdateOwnerAsync(UpdateOwnerDto updateOwnerDto);
}