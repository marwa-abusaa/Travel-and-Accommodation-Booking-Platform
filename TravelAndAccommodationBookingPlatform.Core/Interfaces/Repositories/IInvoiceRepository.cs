using TravelAndAccommodationBookingPlatform.Core.Entities;

namespace TravelAndAccommodationBookingPlatform.Core.Interfaces.Repositories;

public interface IInvoiceRepository
{
    public Task<Invoice> AddInvoiceAsync(Invoice invoice);
    public Task<Invoice?> GetInvoiceByIdAsync(int invoiceId);
    public Task<IEnumerable<Invoice>> GetUserInvoicesForHotelAsync(int userId, int hotelId);
    public Task<IEnumerable<Invoice>> GetAllUserInvoicesAsync(int userId);
}
