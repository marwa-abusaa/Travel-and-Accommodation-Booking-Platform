using Microsoft.EntityFrameworkCore;
using TravelAndAccommodationBookingPlatform.Core.Entities;
using TravelAndAccommodationBookingPlatform.Core.Interfaces.Repositories;
using TravelAndAccommodationBookingPlatform.Core.Models;
using TravelAndAccommodationBookingPlatform.Infrastructure.Data;

namespace TravelAndAccommodationBookingPlatform.Infrastructure.Repositories;

public class RoomRepository : IRoomRepository
{
    private readonly AppDbContext _context;

    public RoomRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Room> AddRoomAsync(Room room)
    {
        var newRoom = await _context.Rooms.AddAsync(room);
        return newRoom.Entity;
    }

    public async Task DeleteRoomByIdAsync(int roomId)
    {
        var room = await GetRoomByIdAsync(roomId);
        if (room != null)
        {
            _context.Rooms.Remove(room);
        }
    }

    public async Task<PaginatedResult<Room>> GetAvailableRoomsByHotelIdAsync(int hotelId, PaginationMetadata pagination)
    {
        pagination.TotalCount = await _context.Rooms.CountAsync();

        if (pagination.PageNumber > pagination.TotalPages && pagination.TotalPages != 0)
            pagination.PageNumber = pagination.TotalPages;

        var skip = (pagination.PageNumber - 1) * pagination.PageSize;

        var query = _context.Rooms.Where(r => r.HotelId == hotelId && r.IsAvailable == true).AsQueryable();

        var items = await query
            .Skip(skip)
            .Take(pagination.PageSize)
            .ToListAsync();

        return new PaginatedResult<Room>(items, pagination);
    }

    public async Task<Room?> GetRoomByIdAsync(int roomId)
    {
        return await _context.Rooms.FindAsync(roomId);
    }

    public async Task<PaginatedResult<Room>> GetRoomsByHotelIdAsync(int hotelId, PaginationMetadata pagination)
    {
        pagination.TotalCount = await _context.Rooms.CountAsync();

        if (pagination.PageNumber > pagination.TotalPages && pagination.TotalPages != 0)
            pagination.PageNumber = pagination.TotalPages;

        var skip = (pagination.PageNumber - 1) * pagination.PageSize;

        var query = _context.Rooms.Where(r => r.HotelId == hotelId).AsQueryable();

        var items = await query
            .Skip(skip)
            .Take(pagination.PageSize)
            .ToListAsync();

        return new PaginatedResult<Room>(items, pagination);
    }

    public async Task<bool> IsRoomAvailableAsync(int roomId)
    {
        var room = await GetRoomByIdAsync(roomId);
        return room != null && room.IsAvailable;
    }

    public Task<IEnumerable<Room>> SearchRoomAsync()
    {
        throw new NotImplementedException();
    }

    public async Task UpdateRoomAsync(Room room)
    {
        if (await GetRoomByIdAsync(room.RoomId) != null)
        {
            _context.Rooms.Update(room);
        }
    }
}
