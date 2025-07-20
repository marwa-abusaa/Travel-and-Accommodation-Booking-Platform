using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Users;
using TravelAndAccommodationBookingPlatform.Application.Interfaces.Commands;
using TravelAndAccommodationBookingPlatform.Application.Interfaces.Queries;
using TravelAndAccommodationBookingPlatform.WebAPI.Dtos.Authentication;

namespace TravelAndAccommodationBookingPlatform.WebAPI.Controllers;

[ApiController]
[Route(("api/auth"))]
public class AuthController : ControllerBase
{
    private readonly IUserCommandService _userCommandService;
    private readonly IUserQueryService _userQueryService;
    private readonly IMapper _mapper;

    public AuthController(
        IUserCommandService userCommandService, 
        IUserQueryService userQueryService, 
        IMapper mapper)
    {
        _userCommandService = userCommandService;
        _userQueryService = userQueryService;
        _mapper = mapper;
    }


    /// <summary>
    /// Registers a new user account.
    /// </summary>
    /// <param name="request">User registration data for sign up.</param>
    /// <returns>A success message if registration is successful.</returns>
    /// <response code="200">User registered successfully.</response>
    /// <response code="400">If the email already exists.</response>
    /// <response code="404">If the specified role is not found.</response>
    [HttpPost("signup")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SignUp([FromBody] SignUpRequestDto request)
    {
        var dto = _mapper.Map<SignUpDto>(request);
        await _userCommandService.SignUpAsync(dto);

        return Ok(new { message = "User registered successfully." });
    }

    /// <summary>
    /// Authenticates a user and returns a JWT token.
    /// </summary>
    /// <param name="request">User login credentials including email and password.</param>
    /// <returns>JWT token string on successful authentication.</returns>
    /// <response code="200">Returns the JWT token string.</response>
    /// <response code="401">If the email or password is incorrect.</response>
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LogInRequestDto request)
    {
        var dto = _mapper.Map<LogInDto>(request);
        var result = await _userQueryService.LogInAsync(dto);

        return Ok(result); 
    }
}
