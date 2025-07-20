using Microsoft.EntityFrameworkCore;
using TravelAndAccommodationBookingPlatform.Core.Entities;
using TravelAndAccommodationBookingPlatform.Core.Interfaces.Repositories;
using TravelAndAccommodationBookingPlatform.Infrastructure.Data;

namespace TravelAndAccommodationBookingPlatform.Infrastructure.Repositories;

public class DiscountRepository : IDiscountRepository
{
    private readonly AppDbContext _context;

    public DiscountRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Discount> AddDiscountAsync(Discount discount)
    {
        var newDiscount = await _context.Discounts.AddAsync(discount);
        return newDiscount.Entity;
    }

    public async Task DeleteDiscountByIdAsync(int discountId)
    {
        var discount = await GetDiscountByIdAsync(discountId);
        if (discount != null)
        {
            _context.Discounts.Remove(discount);
        }
    }

    public async Task<Discount?> GetDiscountByIdAsync(int discountId)
    {
        return await _context.Discounts.FindAsync(discountId);
    }

    public async Task<IEnumerable<Discount>> GetDiscountByRoomIdAsync(int roomId)
    {
        return await _context.Discounts.Where(d => d.RoomId == roomId).ToListAsync();
    }

    public async Task<Discount?> GetBestValidDiscountForRoomAsync(int roomId, DateTime checkIn, DateTime checkOut)
    {
        return await _context.Discounts
        .Where(d =>
            d.RoomId == roomId &&
            d.StartDate < checkOut &&
            d.EndDate > checkIn)
        .OrderByDescending(d => d.Percentage)
        .FirstOrDefaultAsync();
    }

}
