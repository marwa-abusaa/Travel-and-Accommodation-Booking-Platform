using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TravelAndAccommodationBookingPlatform.Core.Entities;
using TravelAndAccommodationBookingPlatform.Core.Interfaces.Repositories;
using TravelAndAccommodationBookingPlatform.Core.Models;
using TravelAndAccommodationBookingPlatform.Infrastructure.Data;

namespace TravelAndAccommodationBookingPlatform.Infrastructure.Repositories;

public class HotelRepository : IHotelRepository
{
    private readonly AppDbContext _context;
    private readonly IBookingRepository _bookingRepository;

    public HotelRepository(AppDbContext context, IBookingRepository bookingRepository)
    {
        _context = context;
        _bookingRepository = bookingRepository;
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

    public async Task<Hotel?> GetHotelByIdAsync(int hotelId)
    {
        return await _context.Hotels
            .Include(h => h.Reviews)
            .Include(h => h.City)
            .Include(h => h.Owner)
            .FirstOrDefaultAsync(h => h.HotelId == hotelId);
    }

    public async Task<PaginatedResult<Hotel>> GetHotelsByCityIdAsync(int cityId, PaginationMetadata pagination)
    {
        pagination.TotalCount = await _context.Hotels.CountAsync();

        if (pagination.PageNumber > pagination.TotalPages && pagination.TotalPages != 0)
            pagination.PageNumber = pagination.TotalPages;

        var skip = (pagination.PageNumber - 1) * pagination.PageSize;

        var query = _context.Hotels
            .Where(h => h.CityId == cityId)
            .Include(h => h.Rooms)
            .Include(h => h.Thumbnail)
            .AsQueryable();

        var items = await query
            .Skip(skip)
            .Take(pagination.PageSize)
            .ToListAsync();

        return new PaginatedResult<Hotel>(items, pagination);
    }

    public async Task<IEnumerable<Hotel>> GetHotelsByOwnerIdAsync(int ownerId)
    {
        return await _context.Hotels
            .Where(h => h.OwnerId == ownerId)
            .Include(h => h.Rooms)
            .Include(h => h.Thumbnail)
            .ToListAsync();
    }

    public async Task<IEnumerable<VisitedHotelDto>> GetRecentVisitedHotelsByUserIdAsync(int userId)
    {
        var bookingData = await _context.Bookings
            .Where(b => b.UserId == userId)
            .Include(b => b.Rooms)
                .ThenInclude(r => r.Hotel)
                    .ThenInclude(h => h.City)
            .Include(b => b.Rooms)
                .ThenInclude(r => r.Hotel)
                    .ThenInclude(h => h.Thumbnail)
            .OrderByDescending(b => b.BookingDate)
            .ToListAsync();


        var recentHotels = bookingData
            .Select(b => new
            {
                Booking = b,
                Hotel = b.Rooms.Select(r => r.Hotel).FirstOrDefault()
            })
            .Where(x => x.Hotel != null)
            .GroupBy(x => x.Hotel.HotelId)
            .Select(g => g.First()) 
            .Take(5)
            .Select(x => new VisitedHotelDto
            {
                HotelId = x.Hotel.HotelId,
                HotelName = x.Hotel.Name,
                CityName = x.Hotel.City.Name,
                StarRating = x.Hotel.StarRating,
                Thumbnail = x.Hotel.Thumbnail,
                TotalPrice = x.Booking.TotalPriceAfterDiscount
            })
            .ToList();

        return recentHotels;
    }

    public IQueryable<Hotel> GetAllAsQueryable()
    {
        return _context.Hotels
            .Include(h=>h.Owner)
            .Include(h => h.Rooms)
                .ThenInclude(r => r.Bookings)
            .AsQueryable();
    }

    public async Task UpdateHotelAsync(Hotel hotel)
    {
        if (await GetHotelByIdAsync(hotel.HotelId) != null)
        {
            _context.Hotels.Update(hotel);
        }
    }

    public async Task<bool> IsHotelExistsAsync(Expression<Func<Hotel, bool>> predicate)
    {
        return await _context.Hotels.AnyAsync(predicate);
    }

    public async Task<List<FeaturedHotelModel>> GetFeaturedDealsAsync(int count)
    {
        var currentDate = DateTime.UtcNow;

        var hotels = await _context.Hotels
            .Include(h => h.Thumbnail)
            .Include(h => h.Rooms)
                .ThenInclude(r => r.Discounts)
            .Where(h => h.Rooms.Any(r => r.Discounts.Any(d => d.StartDate <= currentDate && d.EndDate >= currentDate)))
            .ToListAsync();

        var featuredDeals = hotels.Select(hotel =>
        {
            var originalPrice = hotel.Rooms.Min(r => r.PricePerNight);

            var discountedPrices = hotel.Rooms.Select(r =>
            {
                var maxDiscount = r.Discounts
                    .Where(d => d.StartDate <= currentDate && d.EndDate >= currentDate)
                    .Select(d => d.Percentage)
                    .DefaultIfEmpty(0)
                    .Max();

                return r.PricePerNight * (1 - (maxDiscount / 100m));
            });

            var discountedPrice = discountedPrices.Min();

            return new FeaturedHotelModel
            {
                HotelId = hotel.HotelId,
                HotelName = hotel.Name,
                Location = hotel.Location,
                Thumbnail = hotel.Thumbnail,
                StarRating = hotel.StarRating,
                OriginalPrice = originalPrice,
                DiscountedPrice = discountedPrice
            };
        })
        .OrderBy(h => h.DiscountedPrice)
        .Take(count)
        .ToList();

        return featuredDeals;
    }
}
