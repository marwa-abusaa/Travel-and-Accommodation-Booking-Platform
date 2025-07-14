using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using TravelAndAccommodationBookingPlatform.Core.Entities;
using TravelAndAccommodationBookingPlatform.Core.Exceptions;
using TravelAndAccommodationBookingPlatform.Core.Interfaces.Repositories;
using TravelAndAccommodationBookingPlatform.Core.Interfaces.Services;

namespace TravelAndAccommodationBookingPlatform.Application.Services.Helpers;

public class BookingCreationService : IBookingCreationService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly IDiscountRepository _discountRepository;
    private readonly ILogger<BookingCreationService> _logger;

    public BookingCreationService(
        IUserRepository userRepository,
        IRoomRepository roomRepository, 
        IDiscountRepository discountRepository,
        ILogger<BookingCreationService> logger)
    {
        _userRepository = userRepository;
        _roomRepository = roomRepository;
        _discountRepository = discountRepository;
        _logger = logger;
    }

    public async Task<Booking> CreateBookingAsync(Booking booking)
    {
        var user = await _userRepository.GetUserByIdAsync(booking.UserId);
        if (user is null)
        {
            _logger.LogWarning("User with ID {UserId} not found.", booking.UserId);
            throw new NotFoundException($"User with ID {booking.UserId} not found.");
        }

        if (booking.CheckInDate >= booking.CheckOutDate)
        {
            _logger.LogWarning("Invalid date range: CheckInDate {CheckInDate} >= CheckOutDate {CheckOutDate}", booking.CheckInDate, booking.CheckOutDate);
            throw new ValidationException("Check-out date must be after check-in date.");
        }


        decimal totalBeforeDiscount = 0;
        decimal totalAfterDiscount = 0;
        var validRooms = new List<Room>();


        foreach (var bookedRoom in booking.Rooms)
        {
            var room = await _roomRepository.GetRoomByIdAsync(bookedRoom.RoomId);
            if (room is null)
            {
                _logger.LogWarning("Room with ID {RoomId} not found.", bookedRoom.RoomId);
                throw new ValidationException($"Room with ID {bookedRoom.RoomId} not found.");
            }

            bool isAvailable = await _roomRepository.IsRoomAvailableAsync(bookedRoom.RoomId, booking.CheckInDate, booking.CheckOutDate);

            if (!isAvailable)
            {
                _logger.LogWarning("Room ID {RoomId} is not available from {CheckIn} to {CheckOut}.", bookedRoom.RoomId, booking.CheckInDate, booking.CheckOutDate);
                throw new ConflictException($"Room {bookedRoom.RoomId} is not available for the selected dates.");
            }

            validRooms.Add(room);

            int nights = booking.CheckOutDate.DayNumber - booking.CheckInDate.DayNumber;
            if (nights <= 0)
            {
                _logger.LogWarning("Invalid number of nights: {Nights}", nights);
                throw new ValidationException("Booking must be for at least one night.");
            }

            decimal basePrice = room.PricePerNight * nights;
            totalBeforeDiscount += basePrice;

            var checkInDateTime = booking.CheckInDate.ToDateTime(TimeOnly.MinValue);
            var checkOutDateTime = booking.CheckOutDate.ToDateTime(TimeOnly.MinValue);

            var discount = await _discountRepository.GetBestValidDiscountForRoomAsync(bookedRoom.RoomId, checkInDateTime, checkOutDateTime);

            if (discount != null)
            {
                var discounted = basePrice * (1 - discount.Percentage / 100m);
                totalAfterDiscount += discounted;
            }
            else
            {
                totalAfterDiscount += basePrice;
            }
        }

        var createdBooking = new Booking
        {
            UserId = booking.UserId,
            Rooms = validRooms,
            Remarks = booking.Remarks,
            PaymentType=booking.PaymentType,
            CheckInDate = booking.CheckInDate,
            CheckOutDate = booking.CheckOutDate,
            TotalPriceBeforeDiscount = totalBeforeDiscount,
            TotalPriceAfterDiscount = totalAfterDiscount
        };

        _logger.LogInformation($"Booking created successfully for user {booking.UserId} from {booking.CheckInDate:yyyy-MM-dd} to {booking.CheckOutDate:yyyy-MM-dd}, total: {totalAfterDiscount:C}");

        return createdBooking;
    }
}
