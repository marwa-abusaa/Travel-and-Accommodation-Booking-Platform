using Microsoft.EntityFrameworkCore;
using TravelAndAccommodationBookingPlatform.Core.Entities;
using TravelAndAccommodationBookingPlatform.Core.Interfaces.Repositories;
using TravelAndAccommodationBookingPlatform.Infrastructure.Data;

namespace TravelAndAccommodationBookingPlatform.Infrastructure.Repositories;

public class OwnerRepository : IOwnerRepository
{
    private readonly AppDbContext _context;

    public OwnerRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Owner> AddOwnerAsync(Owner owner)
    {
        var newOwner = await _context.Owners.AddAsync(owner);
        return newOwner.Entity;
    }

    public async Task<Owner?> GetOwnerByHotelIdAsync(int hotelId)
    {
        var hotel = await _context.Hotels
            .Include(h => h.Owner)
            .FirstOrDefaultAsync(h => h.HotelId == hotelId);

        return hotel?.Owner;
    }

    public async Task<Owner?> GetOwnerByIdAsync(int ownerId)
    {
        return await _context.Owners.FindAsync(ownerId);
    }

    public async Task UpdateOwnerAsync(Owner owner)
    {
        if(await GetOwnerByIdAsync(owner.OwnerId) != null)
        {
            _context.Owners.Update(owner);
        }
    }
}
