using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Reviews;
using TravelAndAccommodationBookingPlatform.Application.Interfaces.Commands;
using TravelAndAccommodationBookingPlatform.Application.Interfaces.Queries;
using TravelAndAccommodationBookingPlatform.Core.Models;
using TravelAndAccommodationBookingPlatform.WebAPI.Dtos.Reviews;
using TravelAndAccommodationBookingPlatform.WebAPI.Extensions;
using TravelAndAccommodationBookingPlatform.WebAPI.Shared;

namespace TravelAndAccommodationBookingPlatform.WebAPI.Controllers;

[ApiController]
[Route("api/hotels/{hotelId:int}/reviews")]
[Authorize(Roles = UserRoles.User)]
public class ReviewsController : ControllerBase
{
    private readonly IReviewCommandService _reviewCommandService;
    private readonly IReviewQueryService _reviewQueryService;
    private readonly IMapper _mapper;

    public ReviewsController(
        IReviewCommandService reviewCommandService, 
        IReviewQueryService reviewQueryService,
        IMapper mapper)
    {
        _reviewCommandService = reviewCommandService;
        _reviewQueryService = reviewQueryService;
        _mapper = mapper;
    }


    /// <summary>
    /// Get a specific review by ID.
    /// </summary>
    /// <param name="reviewId">Review ID.</param>
    /// <returns>The review details.</returns>
    /// <response code="200">Returns the review.</response> 
    /// <response code="404">If the hotel is not found.</response> 
    [HttpGet("{reviewId:int}")]
    [ProducesResponseType(typeof(ReviewResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReviewResponseDto>> GetReview(int reviewId)
    {
        var review = await _reviewQueryService.GetReviewByIdAsync(reviewId);
        return Ok(review);
    }


    /// <summary>
    /// Create a new review for a specific hotel.
    /// </summary>
    /// <param name="hotelId">Hotel ID from route.</param>
    /// <param name="request">Review request body.</param>
    /// <returns>The created review.</returns>
    /// <response code="201">Review created successfully.</response>
    /// <response code="404">If the hotel is not found.</response> 
    /// <response code="401">User is not authenticated.</response>
    [HttpPost]
    [ProducesResponseType(typeof(ReviewResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ReviewResponseDto>> CreateReview([FromRoute] int hotelId, [FromBody] ReviewCreationRequestDto request)
    {
        var dto = _mapper.Map<CreateReviewDto>(request);
        dto.UserId = User.GetUserId();
        dto.HotelId = hotelId;
        var createdReview = await _reviewCommandService.AddReviewAsync(dto);

        return CreatedAtAction(nameof(GetReview), new { hotelId=hotelId, reviewId = createdReview.ReviewId }, createdReview);
    }


    /// <summary>
    /// Update a review for a specific hotel.
    /// </summary>
    /// <param name="hotelId">Hotel ID.</param>
    /// <param name="reviewId">Review ID.</param>
    /// <param name="request">Updated review data.</param>
    /// <response code="204">Review updated successfully.</response>
    /// <response code="404">If the hotel/review is not found.</response> 
    /// <response code="401">User is not authenticated.</response>
    [HttpPut("{reviewId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateReview(int reviewId,int hotelId, [FromBody] ReviewUpdateRequestDto request)
    {
        var dto = new UpdateReviewDto
        {
            ReviewId = reviewId,
            HotelId = hotelId,
            UserId = User.GetUserId(),
            Comment = request.Comment,
            Rate = request.Rate
        };

        await _reviewCommandService.UpdateReviewAsync(dto);
        return NoContent();
    }


    /// <summary>
    /// Delete a review by ID for a specific hotel.
    /// </summary>
    /// <param name="reviewId">Review ID.</param>
    /// <response code="204">Review deleted successfully.</response>
    /// <response code="404">If the review is not found.</response> 
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized.</response>
    [HttpDelete("{reviewId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteReview(int reviewId)
    {
        await _reviewCommandService.DeleteReviewByIdAsync(reviewId);
        return NoContent();
    }


    /// <summary>
    /// Get all reviews for a specific hotel (paginated).
    /// </summary>
    /// <param name="hotelId">Hotel ID.</param>
    /// <param name="pageNumber">Page number (default = 1).</param>
    /// <param name="pageSize">Page size (default = 10).</param>
    /// <returns>Paginated list of reviews.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<ReviewResponseDto>), StatusCodes.Status200OK)]
    [AllowAnonymous]
    public async Task<ActionResult<PaginatedResult<ReviewResponseDto>>> GetHotelReviews(
        int hotelId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        if (pageNumber <= 0 || pageSize <= 0)
            return BadRequest("Page number and size must be greater than 0.");

        var pagination = new PaginationMetadata { PageNumber = pageNumber, PageSize = pageSize };

        var hotelReviews = await _reviewQueryService.GetHotelReviewsAsync(hotelId, pagination);
        return Ok(hotelReviews);
    }
}
