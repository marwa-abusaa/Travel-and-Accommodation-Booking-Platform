using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Bookings;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Hotels;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Invoices;
using TravelAndAccommodationBookingPlatform.Application.Interfaces.Queries;
using TravelAndAccommodationBookingPlatform.WebAPI.Extensions;
using TravelAndAccommodationBookingPlatform.WebAPI.Shared;

namespace TravelAndAccommodationBookingPlatform.WebAPI.Controllers;

[ApiController]
[Route("api/user/dashboard")]
[Authorize(Roles = UserRoles.User)]
public class UserDashboardController : ControllerBase
{
    private readonly IBookingQueryService _bookingQueryService;
    private readonly IHotelQueryService _hotelQueryService;
    private readonly IInvoiceQueryService _invoiceQueryService;

    public UserDashboardController(
        IBookingQueryService bookingQueryService,
        IHotelQueryService hotelQueryService,
        IInvoiceQueryService invoiceQueryService)
    {
        _bookingQueryService = bookingQueryService;
        _hotelQueryService = hotelQueryService;
        _invoiceQueryService = invoiceQueryService;
    }


    /// <summary>
    /// Retrieves all bookings made by the current authenticated user.
    /// </summary>
    /// <returns>A list of the user's bookings.</returns>
    /// <response code="200">Returns the list of bookings</response>
    /// <response code="401">User is not authenticated</response>
    /// <response code="403">User is not Unauthorized</response>
    [HttpGet("bookings")]
    [ProducesResponseType(typeof(IEnumerable<BookingResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IEnumerable<BookingResponseDto>>> GetUserBookings()
    {
        int userId = User.GetUserId();
        var userBookings = await _bookingQueryService.GetBookingsByUserIdAsync(userId);
        return Ok(userBookings);
    }


    /// <summary>
    /// Retrieves a list of hotels recently visited by the current user.
    /// </summary>
    /// <returns>A list of recently visited hotels.</returns>
    /// <response code="200">Returns the list of recently visited hotels</response>
    /// <response code="401">User is not authenticated</response>
    /// <response code="403">User is not Unauthorized</response>
    [HttpGet("recent-hotels")]
    [ProducesResponseType(typeof(IEnumerable<RecentHotelDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IEnumerable<RecentHotelDto>>> GetRecentVisitedHotels()
    {
        int userId = User.GetUserId();
        var hotels = await _hotelQueryService.GetRecentVisitedHotelsAsync(userId);
        return Ok(hotels);
    }


    /// <summary>
    /// Retrieves all invoices associated with the current authenticated user.
    /// </summary>
    /// <returns>A list of user invoices.</returns>
    /// <response code="200">Returns the list of invoices</response>
    /// <response code="401">User is not authenticated</response>
    /// <response code="403">User is not Unauthorized</response>
    [HttpGet("invoices")]
    [ProducesResponseType(typeof(IEnumerable<InvoiceResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IEnumerable<InvoiceResponseDto>>> GetUserInvoices()
    {
        int userId = User.GetUserId();
        var invoices = await _invoiceQueryService.GetUserInvoicesByUserIdAsync(userId);
        return Ok(invoices);
    }

}
