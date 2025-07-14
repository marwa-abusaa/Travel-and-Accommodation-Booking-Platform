using TravelAndAccommodationBookingPlatform.Core.Entities;
using TravelAndAccommodationBookingPlatform.Core.Interfaces.Services;

namespace TravelAndAccommodationBookingPlatform.Infrastructure.Services;

public class InvoiceHtmlBuilder : IInvoiceHtmlBuilder
{
    public string BuildInvoiceHtml(Invoice invoice)
    {
        var booking = invoice.Booking;

        var html = $@"
            <html>
              <head>
                <title>Invoice #{invoice.InvoiceId}</title>
              </head>
              <body>
                <h2>Invoice Details</h2>
                <p><strong>Invoice ID:</strong> {invoice.InvoiceId}</p>
                <p><strong>Booking ID:</strong> {booking.BookingId}</p>
                <p><strong>Invoice Date:</strong> {invoice.InvoiceDate:yyyy-MM-dd}</p>
                <p><strong>Payment Status:</strong> {invoice.PaymentStatus}</p>
                <p><strong>Total Amount:</strong> {invoice.TotalAmount:C}</p>
                {(string.IsNullOrWhiteSpace(invoice.Notes) ? "" : $"<p><strong>Notes:</strong> {invoice.Notes}</p>")}

                <h3>Booking Information</h3>
                <p><strong>Check-in:</strong> {booking.CheckInDate}</p>
                <p><strong>Check-out:</strong> {booking.CheckOutDate}</p>
                <p><strong>Booking Date:</strong> {booking.BookingDate}</p>
                <p><strong>Payment Type:</strong> {booking.PaymentType}</p>
                <p><strong>Total Before Discount:</strong> {booking.TotalPriceBeforeDiscount:C}</p>
                <p><strong>Total After Discount:</strong> {booking.TotalPriceAfterDiscount:C}</p>

              </body>
            </html>";

        return html;

    }
}
