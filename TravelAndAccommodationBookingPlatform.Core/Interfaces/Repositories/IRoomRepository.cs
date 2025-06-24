using TravelAndAccommodationBookingPlatform.Core.Entities;
using TravelAndAccommodationBookingPlatform.Core.Enums;
using TravelAndAccommodationBookingPlatform.Core.Models;

namespace TravelAndAccommodationBookingPlatform.Core.Interfaces.Repositories;

public interface IRoomRepository
{
    public Task<Room> AddRoomAsync(Room room);
    public Task<Room?> GetRoomByIdAsync(int roomId);
    public Task DeleteRoomAsync(int roomId);
    public Task UpdateRoomAsync(Room room);
    public Task<PaginatedResult<Room>> GetAllRoomsAsync(int hotelId, PaginationMetadata pagination);
    public Task<IEnumerable<Room>> SearchRoomAsync();
    Task<IEnumerable<Room>> GetRoomsWithRoomClassAsync(RoomClass roomClass);
}
