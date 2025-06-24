using TravelAndAccommodationBookingPlatform.Core.Entities;
using TravelAndAccommodationBookingPlatform.Core.Models;

namespace TravelAndAccommodationBookingPlatform.Core.Interfaces.Repositories;

public interface ICityRepository
{
    public Task<City> AddCityAsync(City city);
    public Task<City?> GetCityByIdAsync(int cityId);
    public Task DeleteCityAsync(int cityId);
    public Task UpdateCityAsync(City city);
    public Task<IEnumerable<City>> GetMostVisitedCitiesAsync(int count);
    public Task<IEnumerable<City>> SearchCityAsync();
    public Task<PaginatedResult<City>> GetAllCities(PaginationMetadata pagination);
}

