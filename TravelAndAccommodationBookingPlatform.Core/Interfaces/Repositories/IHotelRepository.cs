using TravelAndAccommodationBookingPlatform.Core.Entities;
using TravelAndAccommodationBookingPlatform.Core.Models;

namespace TravelAndAccommodationBookingPlatform.Core.Interfaces.Repositories;

public interface IHotelRepository
{
    Task<Hotel> AddHotelAsync(Hotel hotel);
    Task<Hotel?> GetHotelByIdAsync(int hotelId);
    Task<IEnumerable<Hotel>> GetHotelsByOwnerIdAsync(int ownerId);
    Task<PaginatedResult<Hotel>> GetHotelsByCityIdAsync(int cityId, PaginationMetadata pagination);
    Task DeleteHotelByIdAsync(int hotelId);
    Task UpdateHotelAsync(Hotel hotel);
    Task<IEnumerable<Hotel>> SearchHotelAsync();
    Task<PaginatedResult<Hotel>> GetHotelsAsync(PaginationMetadata pagination);
    Task<IEnumerable<Hotel>> GetRecentVisitedHotelsByUserIdAsync(int userId);
}
