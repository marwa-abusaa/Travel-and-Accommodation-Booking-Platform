using AutoMapper;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Owners;
using TravelAndAccommodationBookingPlatform.Core.Entities;

namespace TravelAndAccommodationBookingPlatform.Application.Mapping;

public class OwnerMapper : Profile
{
    public OwnerMapper()
    {
        CreateMap<CreateOwnerDto, Owner>();
        CreateMap<UpdateOwnerDto, Owner>();
        CreateMap<Owner, OwnerResponseDto>();
    }
}
