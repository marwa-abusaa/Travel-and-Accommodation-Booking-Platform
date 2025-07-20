using AutoMapper;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Rooms;
using TravelAndAccommodationBookingPlatform.Core.Entities;

namespace TravelAndAccommodationBookingPlatform.Application.Mapping;

public class RoomProfile : Profile
{
    public RoomProfile()
    {
        CreateMap<CreateRoomDto, Room>();
        CreateMap<UpdateRoomDto, Room>();
        CreateMap<Room, RoomDetailsDto>();
        CreateMap<Room, RoomResponseDto>();
        CreateMap<Room, AdminRoomResponseDto>();
    }
}
