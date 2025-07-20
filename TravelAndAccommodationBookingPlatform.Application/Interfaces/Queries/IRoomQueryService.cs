using Sieve.Models;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Rooms;
using TravelAndAccommodationBookingPlatform.Core.Models;

namespace TravelAndAccommodationBookingPlatform.Application.Interfaces.Queries;

public interface IRoomQueryService
{
    Task<RoomDetailsDto?> GetRoomByIdAsync(int roomId);
    Task<PaginatedResult<AdminRoomResponseDto>> SearchRoomsAsync(SieveModel request);
    Task<PaginatedResult<RoomResponseDto>> GetAvailableRoomsByHotelIdAsync(int hotelId, PaginationMetadata pagination);
}
