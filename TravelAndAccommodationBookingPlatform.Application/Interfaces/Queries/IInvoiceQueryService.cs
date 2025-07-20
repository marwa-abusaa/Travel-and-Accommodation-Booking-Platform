using TravelAndAccommodationBookingPlatform.Application.Dtos.Invoices;

namespace TravelAndAccommodationBookingPlatform.Application.Interfaces.Queries;

public interface IInvoiceQueryService
{
    Task<InvoiceResponseDto?> GetInvoiceByIdAsync(int invoiceId, int currentUserId);
    Task<IEnumerable<InvoiceResponseDto>> GetUserInvoicesByUserIdAsync(int userId);
    Task<byte[]> PrintInvoice(int invoiceId, int currentUserId);
}
