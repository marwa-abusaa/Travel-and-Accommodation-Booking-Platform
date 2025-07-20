using AutoMapper;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Hotels;
using TravelAndAccommodationBookingPlatform.Core.Entities;
using TravelAndAccommodationBookingPlatform.Core.Models;

namespace TravelAndAccommodationBookingPlatform.Application.Mapping;

public class HotelProfile : Profile
{
    public HotelProfile()
    {
        CreateMap<CreateHotelDto, Hotel>();
        CreateMap<UpdateHotelDto, Hotel>();
        CreateMap<Hotel, HotelResponseDto>();
        CreateMap<Hotel, HotelSearchDto>();
        CreateMap<Hotel, AdminHotelResponseDto>();
        CreateMap<VisitedHotelDto, RecentHotelDto>();
    }
}
