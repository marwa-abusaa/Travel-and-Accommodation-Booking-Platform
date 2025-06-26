using Microsoft.EntityFrameworkCore;
using TravelAndAccommodationBookingPlatform.Core.Entities;
using TravelAndAccommodationBookingPlatform.Core.Interfaces.Repositories;
using TravelAndAccommodationBookingPlatform.Core.Models;
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

    public async Task<PaginatedResult<Discount>> GetDiscountsAsync(PaginationMetadata pagination)
    {
        pagination.TotalCount = await _context.Discounts.CountAsync();

        if (pagination.PageNumber > pagination.TotalPages && pagination.TotalPages != 0)
            pagination.PageNumber = pagination.TotalPages;

        var skip = (pagination.PageNumber - 1) * pagination.PageSize;

        var query = _context.Discounts.AsQueryable();

        var items = await query
            .Skip(skip)
            .Take(pagination.PageSize)
            .ToListAsync();

        return new PaginatedResult<Discount>(items, pagination);
    }
}
