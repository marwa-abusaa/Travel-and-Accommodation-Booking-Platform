using System.Linq.Expressions;
using TravelAndAccommodationBookingPlatform.Core.Entities;
using TravelAndAccommodationBookingPlatform.Core.Models;

namespace TravelAndAccommodationBookingPlatform.Core.Interfaces.Repositories;

public interface IRoomRepository
{
    Task<Room> AddRoomAsync(Room room);
    Task<Room?> GetRoomByIdAsync(int roomId);
    Task DeleteRoomByIdAsync(int roomId);
    Task UpdateRoomAsync(Room room);
    IQueryable<Room> GetAllAsQueryable();
    Task<bool> IsRoomAvailableAsync(int roomId, DateOnly fromDate, DateOnly toDate);
    Task<PaginatedResult<Room>> GetAvailableRoomsByHotelIdAsync(int hotelId, PaginationMetadata pagination);
    Task<bool> IsRoomExistsAsync(Expression<Func<Room, bool>> predicate);
}
