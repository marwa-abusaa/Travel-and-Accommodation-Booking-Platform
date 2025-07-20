using System.Linq.Expressions;
using TravelAndAccommodationBookingPlatform.Core.Entities;

namespace TravelAndAccommodationBookingPlatform.Core.Interfaces.Repositories;

public interface ICityRepository
{
    Task<City> AddCityAsync(City city);
    Task<City?> GetCityByIdAsync(int cityId);
    Task DeleteCityByIdAsync(int cityId);
    Task UpdateCityAsync(City city);
    Task<IEnumerable<City>> GetMostVisitedCitiesAsync(int count);
    IQueryable<City> GetAllAsQueryable();
    Task<bool> IsCityExistsAsync(Expression<Func<City, bool>> predicate);
}

