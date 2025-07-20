using AutoMapper;
using Microsoft.Extensions.Logging;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Bookings;
using TravelAndAccommodationBookingPlatform.Application.Interfaces.Queries;
using TravelAndAccommodationBookingPlatform.Core.Exceptions;
using TravelAndAccommodationBookingPlatform.Core.Interfaces.Repositories;

namespace TravelAndAccommodationBookingPlatform.Application.Services.Queries;

public class BookingQueryService : IBookingQueryService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<BookingQueryService> _logger;
    private readonly IMapper _mapper;

    public BookingQueryService(
        IBookingRepository bookingRepository, 
        IUserRepository userRepository, 
        ILogger<BookingQueryService> logger, 
        IMapper mapper)
    {
        _bookingRepository = bookingRepository;
        _userRepository = userRepository;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<BookingResponseDto?> GetBookingByIdAsync(int bookingId)
    {
        var booking = await _bookingRepository.GetBookingByIdAsync(bookingId);
        if (booking is null)
        {
            _logger.LogWarning("Booking with ID {BookingId} was not found.", bookingId);
            throw new NotFoundException($"Booking with ID '{bookingId}' was not found.");
        }

        _logger.LogInformation("Successfully retrieved booking with ID: {BookingId}", bookingId);

        return _mapper.Map<BookingResponseDto>(booking);
    }

    public async Task<IEnumerable<BookingResponseDto>> GetBookingsByUserIdAsync(int userId)
    {
        _logger.LogInformation("Fetching bookings for user with ID: {UserId}", userId);

        var user = await _userRepository.GetUserByIdAsync(userId);
        if (user is null)
        {
            _logger.LogWarning("User with ID {UserId} was not found.", userId);
            throw new NotFoundException($"User with ID '{userId}' was not found.");
        }


        var bookings = await _bookingRepository.GetBookingsByUserIdAsync(userId);

        if (!bookings.Any())
        {
            _logger.LogInformation("No bookings found for user with ID: {UserId}", userId);
            return Enumerable.Empty<BookingResponseDto>();
        }

        _logger.LogInformation("Retrieved {Count} bookings for user ID: {UserId}", bookings.Count(), userId);

        return _mapper.Map<IEnumerable<BookingResponseDto>>(bookings);
    }
}
