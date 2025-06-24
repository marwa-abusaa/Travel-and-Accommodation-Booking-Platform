using TravelAndAccommodationBookingPlatform.Core.Enums;

namespace TravelAndAccommodationBookingPlatform.Core.Entities;

public class Invoice
{
    public int InvoiceId { get; set; }
    public int BookingId { get; set; }
    public Booking Booking { get; set; }
    public DateTime InvoiceDate { get; set; }
    public decimal TotalAmount { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    public string Notes { get; set; } = string.Empty;
}
