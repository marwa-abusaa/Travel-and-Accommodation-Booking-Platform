using TravelAndAccommodationBookingPlatform.Core.Enums;

namespace TravelAndAccommodationBookingPlatform.WebAPI.Dtos.Invoices;

public class InvoiceCreationRequestDto
{
    public DateTime InvoiceDate { get; set; } = DateTime.UtcNow;
    public PaymentStatus PaymentStatus { get; set; }
    public string? Notes { get; set; }
}
