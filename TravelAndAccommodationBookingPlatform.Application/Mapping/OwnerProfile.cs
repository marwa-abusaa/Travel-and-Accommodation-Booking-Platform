using AutoMapper;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Owners;
using TravelAndAccommodationBookingPlatform.Core.Entities;

namespace TravelAndAccommodationBookingPlatform.Application.Mapping;

public class OwnerProfile : Profile
{
    public OwnerProfile()
    {
        CreateMap<CreateOwnerDto, Owner>();
        CreateMap<UpdateOwnerDto, Owner>();
        CreateMap<Owner, OwnerResponseDto>();
    }
}
