using AutoMapper;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Discounts;
using TravelAndAccommodationBookingPlatform.WebAPI.Dtos.Discounts;

namespace TravelAndAccommodationBookingPlatform.WebAPI.Mapping;

public class DiscountProfile : Profile
{
    public DiscountProfile()
    {
        CreateMap<DiscountCreationRequestDto, CreateDiscountDto>();
    }
}
