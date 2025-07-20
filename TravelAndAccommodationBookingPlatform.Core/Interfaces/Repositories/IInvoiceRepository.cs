using TravelAndAccommodationBookingPlatform.Core.Entities;

namespace TravelAndAccommodationBookingPlatform.Core.Interfaces.Repositories;

public interface IInvoiceRepository
{
    Task<Invoice> AddInvoiceAsync(Invoice invoice);
    Task<Invoice?> GetInvoiceByIdAsync(int invoiceId);
    Task<IEnumerable<Invoice>> GetUserInvoicesByUserIdAsync(int userId);
}
