using AutoMapper;
using Microsoft.Extensions.Logging;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Rooms;
using TravelAndAccommodationBookingPlatform.Application.Interfaces.Commands;
using TravelAndAccommodationBookingPlatform.Core.Entities;
using TravelAndAccommodationBookingPlatform.Core.Exceptions;
using TravelAndAccommodationBookingPlatform.Core.Interfaces.Repositories;
using TravelAndAccommodationBookingPlatform.Core.Interfaces.UnitOfWork;

namespace TravelAndAccommodationBookingPlatform.Application.Services.Commands;

public class RoomCommandService : IRoomCommandService
{
    private readonly IRoomRepository _roomRepository;
    private readonly IHotelRepository _hotelRepository;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RoomCommandService> _logger;

    public RoomCommandService(
        IRoomRepository roomRepository,
        IHotelRepository hotelRepository,
        IMapper mapper, IUnitOfWork unitOfWork,
        ILogger<RoomCommandService> logger)
    {
        _roomRepository = roomRepository;
        _hotelRepository = hotelRepository;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<RoomResponseDto> AddRoomAsync(CreateRoomDto createRoomDto)
    {
        _logger.LogInformation("Start adding a new room to hotel ID {HotelId}.", createRoomDto.HotelId);

        if (!await _hotelRepository.IsHotelExistsAsync(h=>h.HotelId==createRoomDto.HotelId))
        {
            _logger.LogWarning("Failed to add room: Hotel with ID {HotelId} not found.", createRoomDto.HotelId);
            throw new NotFoundException($"Hotel with ID '{createRoomDto.HotelId}' not found.");
        }

        var room = _mapper.Map<Room>(createRoomDto);
        var addedRoom = await _roomRepository.AddRoomAsync(room);

        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Room with ID {RoomId} successfully added to hotel ID {HotelId}.", addedRoom.RoomId, createRoomDto.HotelId);
        
        return _mapper.Map<RoomResponseDto>(addedRoom);
    }

    public async Task DeleteRoomByIdAsync(int roomId)
    {
        if (!await _roomRepository.IsRoomExistsAsync(r=>r.RoomId==roomId))
        {
            _logger.LogWarning("Failed to delete: Room with ID {RoomId} not found.", roomId);
            throw new NotFoundException($"Room with ID '{roomId}' not found.");
        }

        await _roomRepository.DeleteRoomByIdAsync(roomId);

        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Room with ID {RoomId} successfully deleted.", roomId);
    }

    public async Task UpdateRoomAsync(UpdateRoomDto updateRoomDto)
    {
        var existingRoom = await _roomRepository.GetRoomByIdAsync(updateRoomDto.RoomId);
        if (existingRoom is null)
        {
            _logger.LogWarning("Failed to update: Room with ID {RoomId} not found.", updateRoomDto.RoomId);
            throw new NotFoundException($"Room with ID '{updateRoomDto.RoomId}' not found.");
        }

        _mapper.Map(updateRoomDto, existingRoom);
        await _roomRepository.UpdateRoomAsync(existingRoom);

        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Room with ID {RoomId} successfully updated.", updateRoomDto.RoomId);
    }
}
