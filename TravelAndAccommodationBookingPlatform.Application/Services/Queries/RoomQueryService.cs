using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sieve.Services;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Discounts;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Images;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Rooms;
using TravelAndAccommodationBookingPlatform.Application.Interfaces.Queries;
using TravelAndAccommodationBookingPlatform.Core.Exceptions;
using TravelAndAccommodationBookingPlatform.Core.Interfaces.Repositories;
using TravelAndAccommodationBookingPlatform.Core.Models;

namespace TravelAndAccommodationBookingPlatform.Application.Services.Queries;

public class RoomQueryService : IRoomQueryService
{
    private readonly IRoomRepository _roomRepository;
    private readonly IHotelRepository _hotelRepository;
    private readonly IImageRepository _imageRepository;
    private readonly IDiscountRepository _discountRepository;
    private readonly ILogger<RoomQueryService> _logger;
    private readonly ISieveProcessor _sieveProcessor;
    private readonly IMapper _mapper;

    public RoomQueryService(
        IRoomRepository roomRepository,
        IHotelRepository hotelRepository,
        IImageRepository imageRepository,
        IDiscountRepository discountRepository, 
        ILogger<RoomQueryService> logger,
        ISieveProcessor sieveProcessor,
        IMapper mapper)
    {
        _roomRepository = roomRepository;
        _hotelRepository = hotelRepository;
        _imageRepository = imageRepository;
        _discountRepository = discountRepository;
        _sieveProcessor = sieveProcessor;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<PaginatedResult<RoomResponseDto>> GetAvailableRoomsByHotelIdAsync(int hotelId, PaginationMetadata pagination)
    {
        _logger.LogInformation("Fetching available rooms for Hotel ID {HotelId}", hotelId);

        if (!await _hotelRepository.IsHotelExistsAsync(h => h.HotelId == hotelId))
        {
            _logger.LogWarning("Failed to get available rooms: Hotel with ID {HotelId} not found.", hotelId);
            throw new NotFoundException($"Hotel with ID '{hotelId}' not found.");
        }

        var rooms = await _roomRepository.GetAvailableRoomsByHotelIdAsync(hotelId, pagination);

        _logger.LogInformation("Retrieved {Count} available rooms for hotel ID {HotelId}.", rooms.Items.Count, hotelId);
        
        return _mapper.Map<PaginatedResult<RoomResponseDto>>(rooms);
    }

    public async Task<RoomDetailsDto?> GetRoomByIdAsync(int roomId)
    {
        var room = await _roomRepository.GetRoomByIdAsync(roomId);
        if (room is null)
        {
            _logger.LogWarning("Room with ID {RoomId} not found.", roomId);
            throw new NotFoundException("The requested room was not found.");
        }

        var roomImages = await _imageRepository.GetRoomImagesAsync(roomId);
        var roomDiscounts = await _discountRepository.GetDiscountByRoomIdAsync(roomId);

        var dto = _mapper.Map<RoomDetailsDto>(room);

        dto.Discounts = _mapper.Map<List<DiscountResponseDto>>(roomDiscounts);
        dto.Images = _mapper.Map<List<ImageResponseDto>>(roomImages);

        _logger.LogInformation("Retrieved room details for room ID {RoomId}.", roomId);

        return dto;
    }

    public async Task<PaginatedResult<AdminRoomResponseDto>> SearchRoomsAsync(AdminRoomSearchRequest request)
    {
        _logger.LogInformation("Admin room search initiated. Page: {Page}, PageSize: {PageSize}",
           request.Page, request.PageSize);

        var today = DateOnly.FromDateTime(DateTime.UtcNow.Date);

        var query = _roomRepository.GetAllAsQueryable()
            .Select(r => new AdminRoomResponseDto
            {
                RoomId = r.RoomId,
                IsAvailable = r.Bookings.All(b => b.CheckOutDate <= today || b.CheckInDate > today),
                AdultCapacity = r.AdultCapacity,
                ChildrenCapacity = r.ChildrenCapacity,
                RoomClass = r.RoomClass,
                CreatedAt = r.CreatedAt,
                UpdatedAt = r.UpdatedAt
            });

        var filtered = _sieveProcessor.Apply(request, query);

        var total = await filtered.CountAsync();
        var data = await filtered.ToListAsync();

        _logger.LogInformation("Admin room search completed. Total matched rooms: {Total}", total);

        return new PaginatedResult<AdminRoomResponseDto>(
            data,
            new PaginationMetadata
            {
                PageNumber = request.Page ?? 1,
                PageSize = request.PageSize ?? 10,
                TotalCount = total
            });
    }

}
