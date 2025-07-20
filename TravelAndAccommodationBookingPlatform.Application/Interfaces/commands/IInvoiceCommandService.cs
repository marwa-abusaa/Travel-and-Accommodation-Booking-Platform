using TravelAndAccommodationBookingPlatform.Application.Dtos.Invoices;

namespace TravelAndAccommodationBookingPlatform.Application.Interfaces.Commands;

public interface IInvoiceCommandService
{
    Task<InvoiceResponseDto> AddInvoiceAsync(CreateInvoiceDto createInvoiceDto);
}