using TravelAndAccommodationBookingPlatform.Application.Dtos.Cities;
using TravelAndAccommodationBookingPlatform.Core.Models;

namespace TravelAndAccommodationBookingPlatform.Application.Interfaces.Queries;

public interface ICityQueryService
{
    Task<CityResponseDto?> GetCityByIdAsync(int cityId);
    Task<IEnumerable<CityResponseDto>> GetMostVisitedCitiesAsync(int count);
    Task<PaginatedResult<AdminCityResponseDto>> SearchCitiesAsync(AdminCitySearchRequest request);
}
