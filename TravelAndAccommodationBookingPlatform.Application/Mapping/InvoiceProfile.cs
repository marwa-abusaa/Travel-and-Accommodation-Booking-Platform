using AutoMapper;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Invoices;
using TravelAndAccommodationBookingPlatform.Core.Entities;

namespace TravelAndAccommodationBookingPlatform.Application.Mapping;

public class InvoiceProfile : Profile
{
    public InvoiceProfile()
    {
        CreateMap<CreateInvoiceDto, Invoice>();
        CreateMap<Invoice, InvoiceResponseDto>();
    }
}
