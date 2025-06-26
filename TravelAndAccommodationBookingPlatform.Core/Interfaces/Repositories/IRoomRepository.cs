using TravelAndAccommodationBookingPlatform.Core.Entities;
using TravelAndAccommodationBookingPlatform.Core.Enums;
using TravelAndAccommodationBookingPlatform.Core.Models;

namespace TravelAndAccommodationBookingPlatform.Core.Interfaces.Repositories;

public interface IRoomRepository
{
    Task<Room> AddRoomAsync(Room room);
    Task<Room?> GetRoomByIdAsync(int roomId);
    Task DeleteRoomByIdAsync(int roomId);
    Task UpdateRoomAsync(Room room);
    Task<PaginatedResult<Room>> GetRoomsByHotelIdAsync(int hotelId, PaginationMetadata pagination);
    Task<IEnumerable<Room>> SearchRoomAsync();
    Task<bool> IsRoomAvailableAsync(int roomId);
    Task<PaginatedResult<Room>> GetAvailableRoomsByHotelIdAsync(int hotelId, PaginationMetadata pagination);
}
