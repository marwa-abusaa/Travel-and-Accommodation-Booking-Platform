using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Hotels;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Owners;
using TravelAndAccommodationBookingPlatform.Application.Interfaces.Commands;
using TravelAndAccommodationBookingPlatform.Application.Interfaces.Queries;
using TravelAndAccommodationBookingPlatform.WebAPI.Dtos.Owners;
using TravelAndAccommodationBookingPlatform.WebAPI.Shared;

namespace TravelAndAccommodationBookingPlatform.WebAPI.Controllers;

[ApiController]
[Route("api/owners")]
[Authorize(Roles = UserRoles.Admin)]
public class OwnersController : ControllerBase
{
    private readonly IOwnerCommandService _ownerCommandService;
    private readonly IOwnerQueryService _ownerQueryService;
    private readonly IHotelQueryService _hotelQueryService;
    private readonly IMapper _mapper;

    public OwnersController(
        IOwnerCommandService ownerCommandService,
        IOwnerQueryService ownerQueryService,
        IHotelQueryService hotelQueryService,
        IMapper mapper)
    {
        _ownerCommandService = ownerCommandService;
        _ownerQueryService = ownerQueryService;
        _hotelQueryService = hotelQueryService;
        _mapper = mapper;
    }


    /// <summary>
    /// Create a new owner.
    /// </summary>
    /// <param name="request">The owner details to create.</param>
    /// <returns>The created owner.</returns>
    /// <response code="201">Returns the newly created owner</response>
    /// <response code="400">If the input is invalid</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized (not an admin).</response>
    [HttpPost]
    [ProducesResponseType(typeof(OwnerResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<OwnerResponseDto>> CreateOwner([FromBody] CreateOwnerDto request)
    {
        var createdOwner = await _ownerCommandService.AddOwnerAsync(request);
        return CreatedAtAction(nameof(GetOwner), new { id = createdOwner.OwnerId }, createdOwner);
    }


    /// <summary>
    /// Update an existing owner.
    /// </summary>
    /// <param name="request">The updated owner information.</param>
    /// <param name="id">The ID of the owner to update.</param>
    /// <response code="204">Owner successfully updated</response>
    /// <response code="404">If the owner is not found</response>
    /// <response code="400">If the input is invalid</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized (not an admin).</response>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateOwner(int id, [FromBody] OwnerUpdateRequestDto request)
    {
        var dto = _mapper.Map<UpdateOwnerDto>(request);
        dto.OwnerId = id;
        await _ownerCommandService.UpdateOwnerAsync(dto);
        return NoContent();
    }


    /// <summary>
    /// Get a specific owner by ID.
    /// </summary>
    /// <param name="id">The ID of the owner to retrieve.</param>
    /// <returns>An OwnerResponseDto object.</returns>
    /// <response code="200">Returns the requested owner</response>
    /// <response code="404">If the owner is not found</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized (not an admin).</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(OwnerResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<OwnerResponseDto>> GetOwner(int id)
    {
        var owner = await _ownerQueryService.GetOwnerByIdAsync(id);
        return Ok(owner);
    }


    /// <summary>
    /// Retrieves all hotels owned by a specific owner.
    /// </summary>
    /// <param name="ownerId">The ID of the hotel owner.</param>
    /// <returns>A list of hotels associated with the specified owner.</returns>
    /// <response code="200">Returns the list of hotels owned by the specified owner.</response>
    /// <response code="404">If the owner is not found</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized (not an admin).</response>
    [HttpGet("{ownerId:int}/hotels")]
    [ProducesResponseType(typeof(IEnumerable<HotelSearchDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<HotelSearchDto>>> GetOwnerHotels(int ownerId)
    {
        var ownerHotels = await _hotelQueryService.GetHotelsByOwnerIdAsync(ownerId);
        return Ok(ownerHotels);
    }

}
