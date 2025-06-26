using Microsoft.EntityFrameworkCore;
using TravelAndAccommodationBookingPlatform.Core.Entities;
using TravelAndAccommodationBookingPlatform.Core.Interfaces.UnitOfWork;
using TravelAndAccommodationBookingPlatform.Infrastructure.Data;

namespace TravelAndAccommodationBookingPlatform.Infrastructure.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync()
    {
        var modified = _context.ChangeTracker.Entries<AuditableEntity>()
                       .Where(e => e.State == EntityState.Modified);

        foreach (var entry in modified)
            entry.Entity.UpdatedAt = DateTime.UtcNow;

        return await _context.SaveChangesAsync();
    }
}
