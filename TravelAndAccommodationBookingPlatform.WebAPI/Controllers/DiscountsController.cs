using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Discounts;
using TravelAndAccommodationBookingPlatform.Application.Interfaces.Commands;
using TravelAndAccommodationBookingPlatform.Application.Interfaces.Queries;
using TravelAndAccommodationBookingPlatform.WebAPI.Dtos.Discounts;
using TravelAndAccommodationBookingPlatform.WebAPI.Shared;

namespace TravelAndAccommodationBookingPlatform.WebAPI.Controllers;

[ApiController]
[Route("api/rooms/{roomId:int}/discounts")]
[Authorize(Roles = UserRoles.Admin)]
public class DiscountsController : ControllerBase
{
    private readonly IDiscountCommandService _discountCommandService;
    private readonly IDiscountQueryService _discountQueryService;
    private readonly IMapper _mapper;

    public DiscountsController(
        IDiscountCommandService discountCommandService,
        IDiscountQueryService discountQueryService,
        IMapper mapper)
    {
        _discountCommandService = discountCommandService;
        _discountQueryService = discountQueryService;
        _mapper = mapper;
    }


    /// <summary>
    /// Get discount by ID.
    /// </summary>
    /// <param name="discountId">Discount ID.</param>
    /// <returns>Discount details.</returns>
    /// <response code="200">Discount found.</response>
    /// <response code="404">Discount not found.</response>
    [HttpGet("{discountId:int}")]
    [ProducesResponseType(typeof(DiscountResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [AllowAnonymous]
    public async Task<ActionResult<DiscountResponseDto>> GetDiscount(int discountId)
    {
        var discount = await _discountQueryService.GetDiscountByIdAsync(discountId);
        return Ok(discount);
    }


    /// <summary>
    /// Create a new discount for a room.
    /// </summary>
    /// <param name="request">Discount data.</param>
    /// <param name="roomId">Room ID.</param>
    /// <returns>The created discount.</returns>
    /// <response code="201">Discount created successfully.</response>
    /// <response code="404">Room not found.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized (not an admin).</response>
    [HttpPost]
    [ProducesResponseType(typeof(DiscountResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<DiscountResponseDto>> CreateDiscount(int roomId, [FromBody] DiscountCreationRequestDto request)
    {
        var dto = _mapper.Map<CreateDiscountDto>(request);
        dto.RoomId = roomId;
        var createdDiscount = await _discountCommandService.AddDiscountAsync(dto);
        return CreatedAtAction(nameof(GetDiscount), new { roomId = roomId, discountId = createdDiscount.DiscountId }, createdDiscount);
    }


    /// <summary>
    /// Delete a discount by ID.
    /// </summary>
    /// <param name="discountId">Discount ID.</param>
    /// <response code="204">Discount deleted.</response>
    /// <response code="404">Discount not found.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized (not an admin).</response>
    [HttpDelete("{discountId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteDiscount(int discountId)
    {
        await _discountCommandService.DeleteDiscountByIdAsync(discountId);
        return NoContent();
    }


    /// <summary>
    /// Get all discounts for a specific room.
    /// </summary>
    /// <param name="roomId">Room ID.</param>
    /// <returns>List of discounts.</returns>
    /// <response code="200">Discounts retrieved successfully.</response>
    /// <response code="404">Room not found.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<DiscountResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<DiscountResponseDto>>> GetRoomDiscounts(int roomId)
    {
        var discounts = await _discountQueryService.GetDiscountsByRoomIdAsync(roomId);
        return Ok(discounts);
    }
}
