using System.Linq.Expressions;
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
    IQueryable<Hotel> GetAllAsQueryable();
    Task<IEnumerable<VisitedHotelDto>> GetRecentVisitedHotelsByUserIdAsync(int userId);
    Task<bool> IsHotelExistsAsync(Expression<Func<Hotel, bool>> predicate);
    Task<List<FeaturedHotelModel>> GetFeaturedDealsAsync(int count);
}
