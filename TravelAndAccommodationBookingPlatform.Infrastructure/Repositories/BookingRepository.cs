using Microsoft.EntityFrameworkCore;
using TravelAndAccommodationBookingPlatform.Core.Entities;
using TravelAndAccommodationBookingPlatform.Core.Interfaces.Repositories;
using TravelAndAccommodationBookingPlatform.Infrastructure.Data;

namespace TravelAndAccommodationBookingPlatform.Infrastructure.Repositories;

public class BookingRepository : IBookingRepository
{
    private readonly AppDbContext _context;

    public BookingRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Booking> AddBookingAsync(Booking booking)
    {
        var newBooking = await _context.Bookings.AddAsync(booking);
        return newBooking.Entity;
    }

    public async Task DeleteBookingByIdAsync(int bookingId)
    {
        var booking = await GetBookingByIdAsync(bookingId);
        if (booking != null)
        {
            _context.Bookings.Remove(booking);
        }
    }

    public async Task<Booking?> GetBookingByIdAsync(int bookingId)
    {
        return await _context.Bookings.Include(b => b.Rooms).FirstOrDefaultAsync(b => b.BookingId == bookingId);
    }

    public async Task<IEnumerable<Booking>> GetBookingsByUserIdAsync(int userId)
    {
        return await _context.Bookings
            .Include(b => b.Rooms)
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.BookingDate)
            .ToListAsync();
    }
}
