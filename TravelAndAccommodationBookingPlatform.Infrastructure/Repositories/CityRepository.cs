using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TravelAndAccommodationBookingPlatform.Core.Entities;
using TravelAndAccommodationBookingPlatform.Core.Interfaces.Repositories;
using TravelAndAccommodationBookingPlatform.Infrastructure.Data;

namespace TravelAndAccommodationBookingPlatform.Infrastructure.Repositories;

public class CityRepository : ICityRepository
{
    private readonly AppDbContext _context;

    public CityRepository(AppDbContext context)
    {
        _context = context;    
    }

    public async Task<City> AddCityAsync(City city)
    {
        var newCity = await _context.Cities.AddAsync(city);
        return newCity.Entity;
    }

    public async Task DeleteCityByIdAsync(int cityId)
    {
        var city = await GetCityByIdAsync(cityId);
        if (city != null)
        {
            _context.Cities.Remove(city);
        }
    }

    public IQueryable<City> GetAllAsQueryable()
    {
        return _context.Cities.Include(c => c.Hotels);
    }

    public async Task<City?> GetCityByIdAsync(int cityId)
    {
        return await _context.Cities.FindAsync(cityId);
    }

    public async Task<IEnumerable<City>> GetMostVisitedCitiesAsync(int count)
    {
        var topCities = await _context.Bookings
            .SelectMany(b => b.Rooms)
            .Where(r => r.Hotel != null && r.Hotel.City != null)
            .GroupBy(r => r.Hotel.CityId)
            .OrderByDescending(g => g.Count())
            .Take(count)
            .Select(g => g.First().Hotel.City)
            .ToListAsync();

        return topCities;
    }

    public async Task<bool> IsCityExistsAsync(Expression<Func<City, bool>> predicate)
    {
        return await _context.Cities.AnyAsync(predicate);
    }

    public async Task UpdateCityAsync(City city)
    {
        if (await GetCityByIdAsync(city.CityId) != null)
        {
            _context.Cities.Update(city);
        }
    }
}
