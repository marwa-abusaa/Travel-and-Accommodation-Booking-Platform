using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sieve.Models;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Cities;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Hotels;
using TravelAndAccommodationBookingPlatform.Application.Interfaces.Commands;
using TravelAndAccommodationBookingPlatform.Application.Interfaces.Queries;
using TravelAndAccommodationBookingPlatform.Core.Models;
using TravelAndAccommodationBookingPlatform.WebAPI.Dtos.Cities;
using TravelAndAccommodationBookingPlatform.WebAPI.Shared;

namespace TravelAndAccommodationBookingPlatform.WebAPI.Controllers;

[ApiController]
[Route("api/cities")]
[Authorize(Roles = UserRoles.Admin)]
public class CitiesController : ControllerBase
{
    private readonly ICityCommandService _cityCommandService;
    private readonly ICityQueryService _cityQueryService;
    private readonly IHotelQueryService _hotelQueryService;
    private readonly IMapper _mapper;

    public CitiesController(
        ICityCommandService cityCommandService,
        ICityQueryService cityQueryService, 
        IHotelQueryService hotelQueryService, 
        IMapper mapper)
    {
        _cityCommandService = cityCommandService;
        _cityQueryService = cityQueryService;
        _hotelQueryService = hotelQueryService;
        _mapper = mapper;
    }


    /// <summary>
    /// Adds a new city to the system.
    /// </summary>
    /// <param name="request">Details of the city to be created.</param>
    /// <returns>The created city details including ID.</returns>
    /// <response code="201">City created successfully.</response>
    /// <response code="409">Conflict if a city with the same post office code already exists.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized (not an admin).</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AdminCityResponseDto>> AddCity([FromBody] CreateCityDto request)
    {
        var createdCity = await _cityCommandService.AddCityAsync(request);
        return CreatedAtAction(nameof(GetCity), new { id = createdCity.CityId }, createdCity);
    }


    /// <summary>
    /// Delete a city by ID.
    /// </summary>
    /// <param name="id">City ID to delete.</param>
    /// <response code="204">City deleted successfully.</response>
    /// <response code="404">If the city is not found.</response> 
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized (not an admin).</response>
    /// <response code="400">Cannot delete city because it has associated hotels.</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteCity(int id)
    {
        await _cityCommandService.DeleteCityByIdAsync(id);
        return NoContent();
    }


    /// <summary>
    /// Update an existing city.
    /// </summary>
    /// <param name="request">Updated city data.</param>
    /// <param name="id">City ID to update.</param>
    /// <response code="204">City updated successfully.</response>
    /// <response code="404">If the city is not found.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized (not an admin).</response>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateCity(int id, [FromBody] CityUpdateRequestDto request)
    {
        var dto = _mapper.Map<UpdateCityDto>(request);
        dto.CityId = id;
        await _cityCommandService.UpdateCityAsync(dto);
        return NoContent();
    }

    /// <summary>
    /// Get a city by ID.
    /// </summary>
    /// <param name="id">City ID.</param>
    /// <returns>The requested city.</returns>
    /// <response code="200">Returns the city.</response>
    /// <response code="404">If the city is not found.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(CityResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [AllowAnonymous]
    public async Task<ActionResult<CityResponseDto>> GetCity(int id)
    {
        var city = await _cityQueryService.GetCityByIdAsync(id);
        return Ok(city);
    }


    /// <summary>
    /// Get top N most visited cities.
    /// </summary>
    /// <param name="count">
    /// (Query parameter) The number of top cities to return. Defaults to 5 if not provided.
    /// </param>
    /// <returns>List of top visited cities.</returns>
    /// <response code="200">Returns top visited cities.</response>
    /// <response code="400">If the count is less than or equal to zero.</response>
    [HttpGet("trending")]
    [ProducesResponseType(typeof(List<CityResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<CityResponseDto>>> GetTrendingCities([FromQuery] int count = 5)
    {
        var cities = await _cityQueryService.GetMostVisitedCitiesAsync(count);
        return Ok(cities);
    }


    /// <summary>
    /// Search cities with filters, sorting, and pagination (for admin).
    /// </summary>
    /// <param name="request">Filtering, sorting, and pagination parameters.</param>
    /// <returns>Paginated list of cities.</returns>
    /// <response code="200">Returns search results.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized (not an admin).</response>
    [HttpGet("search")]
    [ProducesResponseType(typeof(PaginatedResult<AdminCityResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<PaginatedResult<AdminCityResponseDto>>> SearchCities([FromQuery] SieveModel request)
    {
        var result = await _cityQueryService.SearchCitiesAsync(request);
        return Ok(result);
    }


    /// <summary>
    /// Retrieves a paginated list of hotels in a specific city.
    /// </summary>
    /// <param name="cityId">The ID of the city to fetch hotels from.</param>
    /// <param name="pageNumber">The page number for pagination. Defaults to 1.</param>
    /// <param name="pageSize">The number of items per page. Defaults to 10.</param>
    /// <returns>A paginated list of hotels located in the specified city.</returns>
    /// <response code="200">Returns the list of hotels in the specified city.</response>
    /// <response code="400">If the page number or size is less than or equal to zero.</response>
    /// <response code="404">If the specified city with the given ID was not found.</response>
    [HttpGet("{cityId:int}/hotels")]
    [ProducesResponseType(typeof(PaginatedResult<HotelSearchDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [AllowAnonymous]
    public async Task<ActionResult<PaginatedResult<HotelSearchDto>>> GetCityHotels(
    int cityId,
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 10)
    {
        if (pageNumber <= 0 || pageSize <= 0)
            return BadRequest(new { error = "Page number and size must be greater than 0." });

        var pagination = new PaginationMetadata
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var hotels = await _hotelQueryService.GetHotelsByCityIdAsync(cityId, pagination);
        return Ok(hotels);
    }
}
