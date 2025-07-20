using TravelAndAccommodationBookingPlatform.Core.Entities;
using TravelAndAccommodationBookingPlatform.Core.Interfaces.Services;

namespace TravelAndAccommodationBookingPlatform.Application.Services.Helpers;

public class BookingHtmlBuilder : IBookingHtmlBuilder
{
    public string BuildConfirmationHtml(Booking booking)
    {
        var confirmationNumber = $"BK-{booking.BookingId:0000}";
        var hotel = booking.Rooms.First().Hotel;
        var hotelAddress = hotel.Location;
        var roomDescriptions = string.Join(", ", booking.Rooms.Select(r => r.Description));
        var checkIn = booking.CheckInDate;
        var checkOut = booking.CheckOutDate;
        var total = booking.TotalPriceAfterDiscount;

        return $@"
            <html>
              <body>
                <h2>Booking Confirmation</h2>
                <p>Confirmation Number: {confirmationNumber}</p>
                <p>Hotel Address: {hotelAddress}</p>
                <p>Room Details: {roomDescriptions}</p>
                <p>Check-In Date: {checkIn}</p>
                <p>Check-Out Date: {checkOut}</p>
                <p>Total Price: {total}</p>
              </body>
            </html>";
    }
}
