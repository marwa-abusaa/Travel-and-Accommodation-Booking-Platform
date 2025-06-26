using Microsoft.EntityFrameworkCore;
using TravelAndAccommodationBookingPlatform.Core.Entities;
using TravelAndAccommodationBookingPlatform.Core.Interfaces.Repositories;
using TravelAndAccommodationBookingPlatform.Infrastructure.Data;

namespace TravelAndAccommodationBookingPlatform.Infrastructure.Repositories;

public class InvoiceRepository : IInvoiceRepository
{
    private readonly AppDbContext _context;

    public InvoiceRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Invoice> AddInvoiceAsync(Invoice invoice)
    {
        var newInvoice = await _context.Invoices.AddAsync(invoice);
        return newInvoice.Entity;
    }

    public async Task<Invoice?> GetInvoiceByBookingIdAsync(int bookingId)
    {
        return await _context.Invoices.FirstOrDefaultAsync(i => i.BookingId == bookingId);
    }

    public async Task<Invoice?> GetInvoiceByIdAsync(int invoiceId)
    {
        return await _context.Invoices.FindAsync(invoiceId);
    }

    public async Task<IEnumerable<Invoice>> GetUserInvoicesByUserIdAsync(int userId)
    {
        return await _context.Invoices
            .Include(i => i.Booking)
            .Where(i => i.Booking.UserId == userId)
            .ToListAsync();
    }

    public async Task UpdateInvoiceAsync(Invoice invoice)
    {
        if (await GetInvoiceByIdAsync(invoice.InvoiceId) != null)
        {
            _context.Invoices.Update(invoice);
        }
    }
}
