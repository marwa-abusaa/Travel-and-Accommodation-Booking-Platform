using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sieve.Models;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Hotels;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Images;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Owners;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Rooms;
using TravelAndAccommodationBookingPlatform.Application.Interfaces.Commands;
using TravelAndAccommodationBookingPlatform.Application.Interfaces.Queries;
using TravelAndAccommodationBookingPlatform.Core.Models;
using TravelAndAccommodationBookingPlatform.WebAPI.Dtos.Hotels;
using TravelAndAccommodationBookingPlatform.WebAPI.Dtos.Images;
using TravelAndAccommodationBookingPlatform.WebAPI.Shared;

namespace TravelAndAccommodationBookingPlatform.WebAPI.Controllers;

[ApiController]
[Route("api/hotels")]
[Authorize(Roles = UserRoles.Admin)]
public class HotelsController : ControllerBase
{
    private readonly IHotelCommandService _hotelCommandService;
    private readonly IHotelQueryService _hotelQueryService;
    private readonly IImageQueryService _imageQueryService;
    private readonly IImageCommandService _imageCommandService;
    private readonly IOwnerQueryService _ownerQueryService;
    private readonly IRoomQueryService _roomQueryService;
    private readonly IMapper _mapper;

    public HotelsController(
        IHotelCommandService hotelCommandService,
        IHotelQueryService hotelQueryService, 
        IImageQueryService imageQueryService, 
        IImageCommandService imageCommandService,
        IOwnerQueryService ownerQueryService, 
        IRoomQueryService roomQueryService, 
        IMapper mapper)
    {
        _hotelCommandService = hotelCommandService;
        _hotelQueryService = hotelQueryService;
        _imageQueryService = imageQueryService;
        _imageCommandService = imageCommandService;
        _ownerQueryService = ownerQueryService;
        _roomQueryService = roomQueryService;
        _mapper = mapper;
    }


    /// <summary>
    /// Create a new hotel.
    /// </summary>
    /// <param name="request">Hotel creation details.</param>
    /// <returns>Created hotel details.</returns>
    /// <response code="201">Returns the newly created hotel.</response>
    /// <response code="404">City or Owner not found.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized (not an admin).</response>
    /// <response code="400">If the input is invalid</response>
    [HttpPost]
    [ProducesResponseType(typeof(HotelResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<HotelResponseDto>> CreateHotel([FromBody] CreateHotelDto request)
    {
        var createdHotel = await _hotelCommandService.AddHotelAsync(request);

        return CreatedAtAction(nameof(GetHotelDetails), new { id = createdHotel.HotelId }, createdHotel);
    }


    /// <summary>
    /// Delete a hotel by ID.
    /// </summary>
    /// <param name="id">Hotel ID.</param>
    /// <response code="204">Hotel deleted.</response>
    /// <response code="404">Hotel not found.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized (not an admin).</response>
    /// /// <response code="400">Cannot delete hotel because it has associated rooms.</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteHotel(int id)
    {
        await _hotelCommandService.DeleteHotelByIdAsync(id);
        return NoContent();
    }


    /// <summary>
    /// Update a hotel.
    /// </summary>
    /// <param name="request">Hotel update data.</param>
    /// <param name="id">Hotel ID.</param>
    /// <response code="204">Hotel updated successfully.</response>
    /// <response code="404">Hotel not found.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized (not an admin).</response>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateHotel(int id, [FromBody] HotelUpdateRequestDto request)
    {
        var dto = _mapper.Map<UpdateHotelDto>(request);
        dto.HotelId = id;
        await _hotelCommandService.UpdateHotelAsync(dto);
        return NoContent();
    }


    /// <summary>
    /// Retrieves the owner information of a specific hotel.
    /// </summary>
    /// <param name="hotelId">The ID of the hotel whose owner is to be retrieved.</param>
    /// <returns>The owner information of the specified hotel.</returns>
    /// <response code="200">Returns the hotel owner information.</response>
    [HttpGet("{hotelId}/owner")]
    [ProducesResponseType(typeof(OwnerResponseDto), StatusCodes.Status200OK)]
    [AllowAnonymous]  
    public async Task<ActionResult<OwnerResponseDto>> GetHotelOwner(int hotelId)
    {
        var owner = await _ownerQueryService.GetOwnerByHotelIdAsync(hotelId);
        return Ok(owner);
    }


    /// <summary>
    /// Retrieves a specified number of featured hotel deals.
    /// </summary>
    /// <param name="count">The number of featured deals to retrieve. Default is 5.</param>
    /// <returns>A list of featured hotel deals.</returns>
    /// <response code="200">Returns the list of featured hotel deals.</response>
    /// <response code="400">If the count is less than or equal to zero.</response>
    [HttpGet("featured-deals")]
    [ProducesResponseType(typeof(List<FeaturedHotelDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [AllowAnonymous] 
    public async Task<ActionResult<IEnumerable<FeaturedHotelDto>>> GetFeaturedDeals([FromQuery] int count = 5)
    {
        if (count <= 0)
            return BadRequest("Count must be greater than 0.");

        var deals = await _hotelQueryService.GetFeaturedDealsAsync(count);
        return Ok(deals);
    }


    /// <summary>
    /// Get hotel details by ID.
    /// </summary>
    /// <param name="id">Hotel ID.</param>
    /// <returns>Hotel details including images and reviews.</returns>
    /// <response code="200">Returns the hotel details.</response>
    /// <response code="404">Hotel not found.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(HotelDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [AllowAnonymous]
    public async Task<ActionResult<HotelDetailsDto>> GetHotelDetails(int id)
    {
        var hotelDetails = await _hotelQueryService.GetHotelDetailsAsync(id);
        return Ok(hotelDetails);
    }


    /// <summary>
    /// Search hotels with filters and pagination.
    /// </summary>
    /// <param name="request">Search filters.</param>
    /// <returns>Paginated list of hotels.</returns>
    /// <response code="200">Returns a list of hotels.</response>
    [HttpGet("search")]
    [ProducesResponseType(typeof(PaginatedResult<HotelSearchDto>), StatusCodes.Status200OK)]
    [AllowAnonymous]
    public async Task<ActionResult<PaginatedResult<HotelSearchDto>>> SearchHotels([FromQuery] HotelSearchRequest request)
    {
        var result = await _hotelQueryService.SearchHotelsAsync(request);
        return Ok(result);
    }


    /// <summary>
    /// Search hotels for admin with filters.
    /// </summary>
    /// <param name="request">Admin search filters.</param>
    /// <returns>Paginated admin hotel view.</returns>
    /// <response code="200">Returns admin hotel data.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized (not an admin).</response>
    [HttpGet("search-admin")]
    [ProducesResponseType(typeof(PaginatedResult<AdminHotelResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<PaginatedResult<AdminHotelResponseDto>>> SearchHotelsAdmin([FromQuery] SieveModel request)
    {
        var result = await _hotelQueryService.SearchHotelsAdminAsync(request);
        return Ok(result);
    }


    /// <summary>
    /// Retrieves a paginated list of available rooms for a specific hotel.
    /// </summary>
    /// <param name="hotelId">The ID of the hotel to get available rooms for.</param>
    /// <param name="pageNumber">Page number for pagination (default is 1).</param>
    /// <param name="pageSize">Number of items per page for pagination (default is 10).</param>
    /// <returns>A paginated list of available rooms.</returns>
    /// <response code="200">Returns the paginated list of available rooms.</response>
    /// <response code="400">If the page number or page size is less than or equal to zero.</response>
    /// <response code="404">If the hotel is not found.</response>
    [HttpGet("{hotelId:int}/rooms/available")]
    [ProducesResponseType(typeof(PaginatedResult<RoomResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [AllowAnonymous]
    public async Task<ActionResult<PaginatedResult<RoomResponseDto>>> GetAvailableRoomsForHotel(
    int hotelId,
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 10)
    {
        if (pageNumber <= 0 || pageSize <= 0)
            return BadRequest("Page number and size must be greater than 0.");

        var pagination = new PaginationMetadata { PageNumber = pageNumber, PageSize = pageSize };

        var rooms = await _roomQueryService.GetAvailableRoomsByHotelIdAsync(hotelId, pagination);
        return Ok(rooms);
    }



    /// <summary>
    /// Creates a new image for the specified hotel.
    /// </summary>
    /// <param name="hotelId">The ID of the hotel to which the image will be added.</param>
    /// <param name="request">The image details.</param>
    /// <returns>The created image information.</returns>
    /// <response code="201">Image created successfully.</response>
    /// <response code="400">If the request data is invalid.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized (not an admin).</response>
    /// <response code="404">Hotel not found.</response>
    [HttpPost("{hotelId:int}/images")]
    [ProducesResponseType(typeof(ImageResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ImageResponseDto>> CreateHotelImage(int hotelId, [FromBody] HotelImageRequestDto request)
    {
        var dto = _mapper.Map<CreateImageDto>(request);
        dto.HotelId = hotelId;

        var createdImage = await _imageCommandService.AddImageAsync(dto);

        return CreatedAtAction(nameof(GetHotelImage), new { hotelId = hotelId, imageId = createdImage.ImageId }, createdImage);
    }


    /// <summary>
    /// Update an existing image.
    /// </summary>
    /// <param name="hotelId">The ID of the hotel to which the image will be updated.</param>
    /// <param name="imageId">The ID of image to be updated.</param>
    /// <param name="request">The image update details.</param>
    /// <response code="204">Image updated successfully.</response>
    /// <response code="404">Image/hotel not found.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized.</response>
    [HttpPut("{hotelId:int}/images/{imageId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateHotelImage(int hotelId, int imageId, [FromBody] HotelImageRequestDto request)
    {
        var dto = _mapper.Map<UpdateImageDto>(request);
        dto.HotelId = hotelId;
        dto.ImageId = imageId;

        await _imageCommandService.UpdateImageAsync(dto);

        return NoContent();
    }


    /// <summary>
    /// Delete an image by ID.
    /// </summary>
    /// <param name="hotelId">The ID of the hotel to which the image will be deleted.</param>
    /// <param name="imageId">The image ID.</param>
    /// <response code="204">Image deleted successfully.</response>
    /// <response code="404">Image not found.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized.</response>
    [HttpDelete("{hotelId:int}/images/{imageId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteHotelImage(int hotelId, int imageId)
    {
        await _imageCommandService.DeleteImageByIdAsync(imageId);
        return NoContent();
    }


    /// <summary>
    /// Get an image by its ID.
    /// </summary>
    /// <param name="imageId">The image ID.</param>
    /// <returns>The requested image.</returns>
    /// <response code="200">Image retrieved successfully.</response>
    /// <response code="404">Image not found.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized.</response>
    [HttpGet("{hotelId:int}/images/{imageId:int}")]
    [ProducesResponseType(typeof(ImageResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ImageResponseDto>> GetHotelImage(int imageId)
    {
        var image = await _imageQueryService.GetImageByIdAsync(imageId);
        return Ok(image);
    }
}
