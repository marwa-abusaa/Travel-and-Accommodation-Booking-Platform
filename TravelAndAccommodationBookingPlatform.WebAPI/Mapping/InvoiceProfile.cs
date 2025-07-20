using AutoMapper;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Invoices;
using TravelAndAccommodationBookingPlatform.WebAPI.Dtos.Invoices;

namespace TravelAndAccommodationBookingPlatform.WebAPI.Mapping;

public class InvoiceProfile : Profile
{
    public InvoiceProfile()
    {
        CreateMap<InvoiceCreationRequestDto, CreateInvoiceDto>();
    }
}
