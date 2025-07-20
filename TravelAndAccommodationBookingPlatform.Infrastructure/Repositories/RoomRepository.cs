using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
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
        if (pagination.PageNumber > pagination.TotalPages && pagination.TotalPages != 0)
            pagination.PageNumber = pagination.TotalPages;

        var skip = (pagination.PageNumber - 1) * pagination.PageSize;

        var currentDate = DateOnly.FromDateTime(DateTime.UtcNow);

        var query = _context.Rooms
            .Include(r => r.Bookings)
            .Include(r => r.Images)
            .Where(r => r.HotelId == hotelId)
            .Where(r => !r.Bookings.Any(b =>
                b.CheckInDate <= currentDate && b.CheckOutDate > currentDate))
            .OrderBy(r => r.RoomId)
            .AsQueryable();

        pagination.TotalCount = await query.CountAsync();

        var items = await query
            .Skip(skip)
            .Take(pagination.PageSize)
            .ToListAsync();

        return new PaginatedResult<Room>(items, pagination);
    }

    public async Task<Room?> GetRoomByIdAsync(int roomId)
    {
        return await _context.Rooms.Include(r => r.Hotel).FirstOrDefaultAsync(r => r.RoomId == roomId);
    }


    public async Task<bool> IsRoomAvailableAsync(int roomId, DateOnly fromDate, DateOnly toDate)
    {
        var room = await _context.Rooms
            .Include(r => r.Bookings)
            .FirstOrDefaultAsync(r => r.RoomId == roomId);

        if (room == null)
            return false;

        return room.Bookings.All(b =>
            b.CheckOutDate <= fromDate || b.CheckInDate >= toDate);
    }

    public async Task<bool> IsRoomExistsAsync(Expression<Func<Room, bool>> predicate)
    {
        return await _context.Rooms.AnyAsync(predicate);
    }

    public IQueryable<Room> GetAllAsQueryable()
    {
        return _context.Rooms
            .Include(r => r.Bookings);
    }

    public async Task UpdateRoomAsync(Room room)
    {
        if (await GetRoomByIdAsync(room.RoomId) != null)
        {
            _context.Rooms.Update(room);
        }
    }
}