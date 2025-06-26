using Microsoft.EntityFrameworkCore;
using TravelAndAccommodationBookingPlatform.Core.Entities;
using TravelAndAccommodationBookingPlatform.Core.Interfaces.Repositories;
using TravelAndAccommodationBookingPlatform.Core.Models;
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

    public async Task<PaginatedResult<City>> GetCitiesAsync(PaginationMetadata pagination)
    {
        pagination.TotalCount = await _context.Cities.CountAsync();

        if (pagination.PageNumber > pagination.TotalPages && pagination.TotalPages != 0)
            pagination.PageNumber = pagination.TotalPages;

        var skip = (pagination.PageNumber - 1) * pagination.PageSize;

        var query = _context.Cities.AsQueryable();

        var items = await query
            .Skip(skip)
            .Take(pagination.PageSize)
            .ToListAsync();

        return new PaginatedResult<City>(items, pagination);
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

    public async Task<IEnumerable<City>> SearchCityAsync()
    {
        throw new NotImplementedException();
    }

    public async Task UpdateCityAsync(City city)
    {
        if (await GetCityByIdAsync(city.CityId) != null)
        {
            _context.Cities.Update(city);
        }
    }
}
