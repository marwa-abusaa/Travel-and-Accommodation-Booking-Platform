using TravelAndAccommodationBookingPlatform.Application.Dtos.Owners;

namespace TravelAndAccommodationBookingPlatform.Application.Interfaces.Queries;

public interface IOwnerQueryService
{
    Task<OwnerResponseDto?> GetOwnerByIdAsync(int ownerId);
    Task<OwnerResponseDto?> GetOwnerByHotelIdAsync(int hotelId);
}
