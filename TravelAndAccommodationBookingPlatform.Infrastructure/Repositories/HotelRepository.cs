using Microsoft.EntityFrameworkCore;
using TravelAndAccommodationBookingPlatform.Core.Entities;
using TravelAndAccommodationBookingPlatform.Core.Interfaces.Repositories;
using TravelAndAccommodationBookingPlatform.Core.Models;
using TravelAndAccommodationBookingPlatform.Infrastructure.Data;

namespace TravelAndAccommodationBookingPlatform.Infrastructure.Repositories;

public class HotelRepository : IHotelRepository
{
    private readonly AppDbContext _context;

    public HotelRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Hotel> AddHotelAsync(Hotel hotel)
    {
        var newHotel = await _context.Hotels.AddAsync(hotel);
        return newHotel.Entity;
    }

    public async Task DeleteHotelByIdAsync(int hotelId)
    {
        var hotel = await GetHotelByIdAsync(hotelId);
        if (hotel != null)
        {
            _context.Hotels.Remove(hotel);
        }
    }

    public async Task<PaginatedResult<Hotel>> GetHotelsAsync(PaginationMetadata pagination)
    {
        pagination.TotalCount = await _context.Hotels.CountAsync();

        if (pagination.PageNumber > pagination.TotalPages && pagination.TotalPages != 0)
            pagination.PageNumber = pagination.TotalPages;

        var skip = (pagination.PageNumber - 1) * pagination.PageSize;

        var query = _context.Hotels.AsQueryable();

        var items = await query
            .Skip(skip)
            .Take(pagination.PageSize)
            .ToListAsync();

        return new PaginatedResult<Hotel>(items, pagination);
    }

    public async Task<Hotel?> GetHotelByIdAsync(int hotelId)
    {
        return await _context.Hotels.FindAsync(hotelId);
    }

    public async Task<PaginatedResult<Hotel>> GetHotelsByCityIdAsync(int cityId, PaginationMetadata pagination)
    {
        pagination.TotalCount = await _context.Hotels.CountAsync();

        if (pagination.PageNumber > pagination.TotalPages && pagination.TotalPages != 0)
            pagination.PageNumber = pagination.TotalPages;

        var skip = (pagination.PageNumber - 1) * pagination.PageSize;

        var query = _context.Hotels.Where(h => h.CityId == cityId).AsQueryable();

        var items = await query
            .Skip(skip)
            .Take(pagination.PageSize)
            .ToListAsync();

        return new PaginatedResult<Hotel>(items, pagination);
    }

    public async Task<IEnumerable<Hotel>> GetHotelsByOwnerIdAsync(int ownerId)
    {
        return await _context.Hotels.Where(h => h.OwnerId == ownerId).ToListAsync();
    }

    public async Task<IEnumerable<Hotel>> GetRecentVisitedHotelsByUserIdAsync(int userId)
    {
        var recentHotels = await _context.Bookings
            .Where(b=>b.UserId==userId)
            .OrderByDescending(b=>b.BookingDate)
            .SelectMany(b => b.Rooms.Select(r => r.Hotel))
            .Distinct()
            .Take(5)
            .ToListAsync();

        return recentHotels;
    }

    public async Task<IEnumerable<Hotel>> SearchHotelAsync()
    {
        throw new NotImplementedException();
    }

    public async Task UpdateHotelAsync(Hotel hotel)
    {
        if (await GetHotelByIdAsync(hotel.HotelId) != null)
        {
            _context.Hotels.Update(hotel);
        }
    }
}
