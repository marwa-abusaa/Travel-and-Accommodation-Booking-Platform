using TravelAndAccommodationBookingPlatform.Core.Entities;

namespace TravelAndAccommodationBookingPlatform.Core.Interfaces.Services;

public interface IInvoiceHtmlBuilder
{
    string BuildInvoiceHtml(Invoice invoice);
}
