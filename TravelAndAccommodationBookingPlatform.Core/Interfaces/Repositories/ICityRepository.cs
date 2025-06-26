using TravelAndAccommodationBookingPlatform.Core.Entities;
using TravelAndAccommodationBookingPlatform.Core.Models;

namespace TravelAndAccommodationBookingPlatform.Core.Interfaces.Repositories;

public interface ICityRepository
{
    Task<City> AddCityAsync(City city);
    Task<City?> GetCityByIdAsync(int cityId);
    Task DeleteCityByIdAsync(int cityId);
    Task UpdateCityAsync(City city);
    Task<IEnumerable<City>> GetMostVisitedCitiesAsync(int count);
    Task<IEnumerable<City>> SearchCityAsync();
    Task<PaginatedResult<City>> GetCitiesAsync(PaginationMetadata pagination);
}

