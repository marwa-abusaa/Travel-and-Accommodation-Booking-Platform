using TravelAndAccommodationBookingPlatform.Application.Dtos.Invoices;

namespace TravelAndAccommodationBookingPlatform.Application.Interfaces.Queries;

public interface IInvoiceQueryService
{
    Task<InvoiceResponseDto?> GetInvoiceByIdAsync(int invoiceId);
    Task<InvoiceResponseDto?> GetInvoiceByBookingIdAsync(int bookingId);
    Task<IEnumerable<InvoiceResponseDto>> GetUserInvoicesByUserIdAsync(int userId);
    Task<byte[]> PrintInvoice(int invoiceId);
}
