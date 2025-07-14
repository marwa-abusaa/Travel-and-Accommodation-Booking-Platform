using TravelAndAccommodationBookingPlatform.Core.Enums;

namespace TravelAndAccommodationBookingPlatform.Application.Dtos.Invoices;

public class InvoiceResponseDto
{
    public int InvoiceId { get; set; }
    public int BookingId { get; set; }
    public DateTime InvoiceDate { get; set; } = DateTime.UtcNow;
    public decimal TotalAmount { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    public string? Notes { get; set; } = string.Empty;
}
