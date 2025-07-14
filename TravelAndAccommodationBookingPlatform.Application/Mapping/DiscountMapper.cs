using AutoMapper;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Discounts;
using TravelAndAccommodationBookingPlatform.Core.Entities;

namespace TravelAndAccommodationBookingPlatform.Application.Mapping;

public class DiscountMapper : Profile
{
    public DiscountMapper()
    {
        CreateMap<CreateDiscountDto, Discount>();
        CreateMap<Discount, DiscountResponseDto>();
    }
}
