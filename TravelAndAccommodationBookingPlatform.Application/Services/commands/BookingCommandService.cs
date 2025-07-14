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
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<BookingCommandService> _logger;
    private readonly IMapper _mapper;

    public BookingCommandService(
        IBookingRepository bookingRepository,
        IBookingCreationService bookingCreationService,
        IBookingConfirmationService bookingConfirmationService, 
        IUnitOfWork unitOfWork,
        ILogger<BookingCommandService> logger,
        IMapper mapper)
    {
        _bookingRepository = bookingRepository;
        _bookingCreationService = bookingCreationService;
        _bookingConfirmationService = bookingConfirmationService;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<BookingResponseDto> AddBookingAsync(CreateBookingDto createBookingDto)
    {
        _logger.LogInformation("Starting booking process for user ID {UserId}", createBookingDto.UserId);

        var bookingToCreate = _mapper.Map<Booking>(createBookingDto);
        var booking = await _bookingCreationService.CreateBookingAsync(bookingToCreate);
        await _bookingRepository.AddBookingAsync(booking);
        await _unitOfWork.SaveChangesAsync();
        await _bookingConfirmationService.SendBookingConfirmationAsync(booking);
        return _mapper.Map<BookingResponseDto>(booking);
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
