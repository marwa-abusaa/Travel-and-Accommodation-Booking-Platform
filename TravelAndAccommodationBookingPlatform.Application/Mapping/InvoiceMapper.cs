using AutoMapper;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Invoices;
using TravelAndAccommodationBookingPlatform.Core.Entities;

namespace TravelAndAccommodationBookingPlatform.Application.Mapping;

public class InvoiceMapper : Profile
{
    public InvoiceMapper()
    {
        CreateMap<CreateInvoiceDto, Invoice>();
        CreateMap<UpdateInvoiceDto, Invoice>();
        CreateMap<Invoice, InvoiceResponseDto>();
    }
}
