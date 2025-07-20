using AutoMapper;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Rooms;
using TravelAndAccommodationBookingPlatform.WebAPI.Dtos.Rooms;

namespace TravelAndAccommodationBookingPlatform.WebAPI.Mapping;

public class RoomProfile : Profile
{
    public RoomProfile()
    {
        CreateMap<RoomUpdateRequestDto, UpdateRoomDto>();
    }
}
