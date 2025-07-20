using TravelAndAccommodationBookingPlatform.Application.Dtos.Rooms;

namespace TravelAndAccommodationBookingPlatform.Application.Interfaces.Commands;

public interface IRoomCommandService
{
    Task<RoomResponseDto> AddRoomAsync(CreateRoomDto createRoomDto);
    Task DeleteRoomByIdAsync(int roomId);
    Task UpdateRoomAsync(UpdateRoomDto updateRoomDto);
}