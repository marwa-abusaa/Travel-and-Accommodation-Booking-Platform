using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sieve.Models;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Images;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Rooms;
using TravelAndAccommodationBookingPlatform.Application.Interfaces.Commands;
using TravelAndAccommodationBookingPlatform.Application.Interfaces.Queries;
using TravelAndAccommodationBookingPlatform.Core.Models;
using TravelAndAccommodationBookingPlatform.WebAPI.Dtos.Images;
using TravelAndAccommodationBookingPlatform.WebAPI.Dtos.Rooms;
using TravelAndAccommodationBookingPlatform.WebAPI.Shared;

namespace TravelAndAccommodationBookingPlatform.WebAPI.Controllers;

[ApiController]
[Route("api/rooms")]
[Authorize(Roles = UserRoles.Admin)]
public class RoomsController : ControllerBase
{
    private readonly IRoomCommandService _roomCommandService;
    private readonly IRoomQueryService _roomQueryService;
    private readonly IImageQueryService _imageQueryService;
    private readonly IImageCommandService _imageCommandService;
    private readonly IMapper _mapper;

    public RoomsController(
        IRoomCommandService roomCommandService,
        IRoomQueryService roomQueryService, 
        IImageQueryService imageQueryService, 
        IImageCommandService imageCommandService,
        IMapper mapper)
    {
        _roomCommandService = roomCommandService;
        _roomQueryService = roomQueryService;
        _imageQueryService = imageQueryService;
        _imageCommandService = imageCommandService;
        _mapper = mapper;
    }


    /// <summary>
    /// Get a room by ID.
    /// </summary>
    /// <param name="id">Room ID.</param>
    /// <returns>The requested room.</returns>
    /// <response code="200">Room retrieved successfully.</response>
    /// <response code="404">Room not found.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(RoomResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [AllowAnonymous]
    public async Task<ActionResult<RoomDetailsDto>> GetRoom(int id)
    {
        var roomDetails = await _roomQueryService.GetRoomByIdAsync(id);
        return Ok(roomDetails);
    }


    /// <summary>
    /// Create a new room.
    /// </summary>
    /// <param name="request">The room details.</param>
    /// <returns>The created room.</returns>
    /// <response code="201">Room created successfully.</response>
    /// <response code="404">Hotel not found.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized (not an admin).</response>
    [HttpPost]
    [ProducesResponseType(typeof(RoomResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<RoomResponseDto>> CreateRoom([FromBody] CreateRoomDto request)
    {
        var createdRoom = await _roomCommandService.AddRoomAsync(request);

        return CreatedAtAction(nameof(GetRoom), new { id = createdRoom.RoomId }, createdRoom);
    }

    /// <summary>
    /// Delete a room by ID.
    /// </summary>
    /// <param name="id">Room ID.</param>
    /// <response code="204">Room deleted successfully.</response>
    /// <response code="404">Room not found.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized (not an admin).</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteRoom(int id)
    {
        await _roomCommandService.DeleteRoomByIdAsync(id);
        return NoContent();
    }

    /// <summary>
    /// Update an existing room.
    /// </summary>
    /// <param name="request">Updated room details.</param>
    /// <param name="id">Room ID.</param>
    /// <response code="204">Room updated successfully.</response>
    /// <response code="404">Room not found.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized (not an admin).</response>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateRoom(int id, [FromBody] RoomUpdateRequestDto request)
    {
        var dto = _mapper.Map<UpdateRoomDto>(request);
        dto.RoomId = id;
        await _roomCommandService.UpdateRoomAsync(dto);
        return NoContent();
    }


    /// <summary>
    /// Search for rooms with filtering and pagination (admin only).
    /// </summary>
    /// <param name="request">Search and pagination parameters.</param>
    /// <returns>Paginated list of rooms.</returns>
    /// <response code="200">Rooms retrieved successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized (not an admin).</response>
    [HttpGet("search")]
    [ProducesResponseType(typeof(PaginatedResult<AdminRoomResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<PaginatedResult<AdminRoomResponseDto>>> SearchRooms([FromQuery] SieveModel request)
    {
        var searchResult = await _roomQueryService.SearchRoomsAsync(request);
        return Ok(searchResult);
    }


    /// <summary>
    /// Creates a new image for the specified hotel.
    /// </summary>
    /// <param name="roomId">The ID of the room to which the image will be added.</param>
    /// <param name="request">The image details.</param>
    /// <returns>The created image information.</returns>
    /// <response code="201">Image created successfully.</response>
    /// <response code="400">If the request data is invalid.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized (not an admin).</response>
    /// <response code="404">Room not found.</response>
    [HttpPost("{roomId:int}/images")]
    [ProducesResponseType(typeof(ImageResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ImageResponseDto>> CreateRoomImage(int roomId, [FromBody] RoomImageRequestDto request)
    {
        var dto = _mapper.Map<CreateImageDto>(request);
        dto.RoomId = roomId;

        var createdImage = await _imageCommandService.AddImageAsync(dto);

        return CreatedAtAction(nameof(GetRoomImage), new { roomId= roomId ,imageId = createdImage.ImageId }, createdImage);
    }


    /// <summary>
    /// Update an existing image.
    /// </summary>
    /// <param name="roomId">The ID of the room to which the image will be updated.</param>
    /// <param name="imageId">The ID of image to be updated.</param>
    /// <param name="request">The image update details.</param>
    /// <response code="204">Image updated successfully.</response>
    /// <response code="404">Image/room not found.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized.</response>
    [HttpPut("{roomId:int}/images/{imageId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateRoomImage(int roomId, int imageId, [FromBody] RoomImageRequestDto request)
    {
        var dto = _mapper.Map<UpdateImageDto>(request);
        dto.RoomId = roomId;
        dto.ImageId = imageId;

        await _imageCommandService.UpdateImageAsync(dto);

        return NoContent();
    }


    /// <summary>
    /// Delete an image by ID.
    /// </summary>
    /// <param name="roomId">The ID of the room to which the image will be deleted.</param>
    /// <param name="imageId">The image ID.</param>
    /// <response code="204">Image deleted successfully.</response>
    /// <response code="404">Image not found.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized.</response>
    [HttpDelete("{roomId:int}/images/{imageId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteRoomImage(int roomId, int imageId)
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
    [HttpGet("{roomId:int}/images/{imageId:int}")]
    [ProducesResponseType(typeof(ImageResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ImageResponseDto>> GetRoomImage(int imageId)
    {
        var image = await _imageQueryService.GetImageByIdAsync(imageId);
        return Ok(image);
    }

}
