using AutoMapper;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Owners;
using TravelAndAccommodationBookingPlatform.WebAPI.Dtos.Owners;

namespace TravelAndAccommodationBookingPlatform.WebAPI.Mapping;

public class OwnerProfile : Profile
{
    public OwnerProfile()
    {
        CreateMap<OwnerUpdateRequestDto, UpdateOwnerDto>();
    }
}
