using AutoMapper;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Rooms;
using TravelAndAccommodationBookingPlatform.Core.Entities;

namespace TravelAndAccommodationBookingPlatform.Application.Mapping;

public class RoomMapper : Profile
{
    public RoomMapper()
    {
        CreateMap<CreateRoomDto, Room>();
        CreateMap<UpdateRoomDto, Room>();
        CreateMap<Room, RoomDetailsDto>();
        CreateMap<Room, RoomResponseDto>();
    }
}
