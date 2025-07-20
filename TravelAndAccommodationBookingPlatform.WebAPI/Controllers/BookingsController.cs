using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Bookings;
using TravelAndAccommodationBookingPlatform.Application.Interfaces.Commands;
using TravelAndAccommodationBookingPlatform.Application.Interfaces.Queries;
using TravelAndAccommodationBookingPlatform.WebAPI.Dtos.Bookings;
using TravelAndAccommodationBookingPlatform.WebAPI.Extensions;
using TravelAndAccommodationBookingPlatform.WebAPI.Shared;

namespace TravelAndAccommodationBookingPlatform.WebAPI.Controllers;

[ApiController]
[Route("api/bookings")]
[Authorize(Roles = UserRoles.User)]
public class BookingsController : ControllerBase
{
    private readonly IBookingCommandService _bookingCommandService;
    private readonly IBookingQueryService _bookingQueryService;
    private readonly IMapper _mapper;

    public BookingsController(
        IBookingCommandService bookingCommandService,
        IBookingQueryService bookingQueryService, 
        IMapper mapper)
    {
        _bookingCommandService = bookingCommandService;
        _bookingQueryService = bookingQueryService;
        _mapper = mapper;
    }


    /// <summary>
    /// Create a new booking for the authenticated user.
    /// </summary>
    /// <param name="request">Booking creation details including room IDs, check-in/out dates, PaymentType and remarks.</param>
    /// <returns>The created booking details.</returns>
    /// <response code="201">Booking created successfully.</response>
    /// <response code="400">Invalid input data.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="404">One or more rooms not found.</response>
    [HttpPost]
    [ProducesResponseType(typeof(BookingResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<BookingResponseDto>> CreateBooking([FromBody] BookingCreationRequestDto request)
    {
        var dto = _mapper.Map<CreateBookingDto>(request);
        dto.UserId = User.GetUserId();

        var createdBooking = await _bookingCommandService.AddBookingAsync(dto);
        return CreatedAtAction(nameof(GetBooking), new { id = createdBooking.BookingId }, createdBooking);
    }

    /// <summary>
    /// Delete a booking by ID for the authenticated user.
    /// </summary>
    /// <param name="id">Booking ID to delete.</param>
    /// <returns>No content.</returns>
    /// <response code="204">Booking deleted successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized to delete this booking.</response>
    /// <response code="404">Booking not found.</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteBooking(int id)
    {
        await _bookingCommandService.DeleteBookingByIdAsync(id);
        return NoContent();
    }

    /// <summary>
    /// Get booking details by booking ID for the authenticated user.
    /// </summary>
    /// <param name="id">Booking ID.</param>
    /// <returns>Booking details.</returns>
    /// <response code="200">Booking retrieved successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="404">Booking not found.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(BookingResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<BookingResponseDto>> GetBooking(int id)
    {
        var booking = await _bookingQueryService.GetBookingByIdAsync(id);
        return Ok(booking);
    }

}
