using TravelAndAccommodationBookingPlatform.Core.Entities;
using TravelAndAccommodationBookingPlatform.Core.Models;

namespace TravelAndAccommodationBookingPlatform.Core.Interfaces.Repositories;

public interface IHotelRepository
{
    public Task<Hotel> AddHotelAsync(Hotel hotel);
    public Task<Hotel?> GetHotelByIdAsync(int hotelId);
    public Task<IEnumerable<Hotel>> GetHotelsByOwnerIdAsync(int ownerId);
    public Task<PaginatedResult<Hotel>> GetHotelsByCityIdAsync(int cityId, PaginationMetadata pagination);
    public Task DeleteHotelAsync(int hotelId);
    public Task UpdateHotelAsync(Hotel hotel);
    public Task<IEnumerable<Hotel>> SearchHotelAsync();
    public Task<PaginatedResult<City>> GetAllHotels(PaginationMetadata pagination);
    public Task<IEnumerable<Hotel>> GetRecentVisitedHotelsByUserAsync(int userId);
}
