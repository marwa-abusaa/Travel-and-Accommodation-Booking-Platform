using AutoMapper;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Hotels;
using TravelAndAccommodationBookingPlatform.Core.Entities;

namespace TravelAndAccommodationBookingPlatform.Application.Mapping;

public class HotelMapper : Profile
{
    public HotelMapper()
    {
        CreateMap<CreateHotelDto, Hotel>();
        CreateMap<UpdateHotelDto, Hotel>();
        CreateMap<Hotel, HotelResponseDto>();
        CreateMap<Hotel, HotelSearchDto>();
    }
}
