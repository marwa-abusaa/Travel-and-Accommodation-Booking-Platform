using AutoMapper;
using Microsoft.Extensions.Logging;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Bookings;
using TravelAndAccommodationBookingPlatform.Application.Interfaces.Commands;
using TravelAndAccommodationBookingPlatform.Core.Entities;
using TravelAndAccommodationBookingPlatform.Core.Exceptions;
using TravelAndAccommodationBookingPlatform.Core.Interfaces.Repositories;
using TravelAndAccommodationBookingPlatform.Core.Interfaces.Services;
using TravelAndAccommodationBookingPlatform.Core.Interfaces.UnitOfWork;

namespace TravelAndAccommodationBookingPlatform.Application.Services.Commands;

public class BookingCommandService : IBookingCommandService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IBookingCreationService _bookingCreationService;
    private readonly IBookingConfirmationService _bookingConfirmationService;
    private readonly IRoomRepository _roomRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<BookingCommandService> _logger;
    private readonly IMapper _mapper;

    public BookingCommandService(
        IBookingRepository bookingRepository,
        IBookingCreationService bookingCreationService,
        IBookingConfirmationService bookingConfirmationService,
        IRoomRepository roomRepository,
        IUnitOfWork unitOfWork,
        ILogger<BookingCommandService> logger,
        IMapper mapper)
    {
        _bookingRepository = bookingRepository;
        _bookingCreationService = bookingCreationService;
        _bookingConfirmationService = bookingConfirmationService;
        _roomRepository = roomRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<BookingResponseDto> AddBookingAsync(CreateBookingDto dto)
    {
        _logger.LogInformation("Starting booking process for user ID {UserId}", dto.UserId);

        var rooms = await GetRoomsByIdsAsync(dto.RoomIds);

        var bookingToCreate = new Booking
        {
            UserId = dto.UserId,
            Rooms = rooms,
            Remarks = dto.Remarks ?? string.Empty,
            CheckInDate = dto.CheckInDate,
            CheckOutDate = dto.CheckOutDate,
            PaymentType=dto.PaymentType
        };

        var booking = await _bookingCreationService.CreateBookingAsync(bookingToCreate);
        await _bookingRepository.AddBookingAsync(booking);
        await _unitOfWork.SaveChangesAsync();
        await _bookingConfirmationService.SendBookingConfirmationAsync(booking);
        return _mapper.Map<BookingResponseDto>(booking);
    }

    private async Task<List<Room>> GetRoomsByIdsAsync(IEnumerable<int> roomIds)
    {
        var rooms = new List<Room>();

        foreach (var roomId in roomIds)
        {
            var room = await _roomRepository.GetRoomByIdAsync(roomId);
            if (room is null)
                throw new NotFoundException($"Room with ID {roomId} not found");
            rooms.Add(room);
        }
        return rooms;
    }

    public async Task DeleteBookingByIdAsync(int bookingId)
    {
        var booking = await _bookingRepository.GetBookingByIdAsync(bookingId);
        if (booking is null)
        {
            _logger.LogWarning("Booking with ID {BookingId} not found", bookingId);
            throw new NotFoundException($"Booking with ID {bookingId} not found.");
        }

        await _bookingRepository.DeleteBookingByIdAsync(bookingId);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Booking with ID {BookingId} deleted successfully", bookingId);
    }
}
