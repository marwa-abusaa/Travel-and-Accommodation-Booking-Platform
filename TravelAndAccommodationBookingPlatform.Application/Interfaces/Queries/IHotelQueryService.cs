using TravelAndAccommodationBookingPlatform.Application.Dtos.Hotels;
using TravelAndAccommodationBookingPlatform.Core.Models;

namespace TravelAndAccommodationBookingPlatform.Application.Interfaces.Queries;

public interface IHotelQueryService
{
    Task<HotelDetailsDto?> GetHotelDetailsAsync(int hotelId);
    Task<IEnumerable<HotelSearchDto>> GetHotelsByOwnerIdAsync(int ownerId);
    Task<PaginatedResult<HotelSearchDto>> GetHotelsByCityIdAsync(int cityId, PaginationMetadata pagination);
    Task<PaginatedResult<HotelSearchDto>> SearchHotelsAsync(HotelSearchRequest request);
    Task<PaginatedResult<AdminHotelResponseDto>> SearchHotelsAdminAsync(AdminHotelSearchRequest request);
    Task<IEnumerable<RecentHotelDto>> GetRecentVisitedHotelsAsync(int userId);
    Task<IEnumerable<FeaturedHotelDto>> GetFeaturedDealsAsync(int count);
}
