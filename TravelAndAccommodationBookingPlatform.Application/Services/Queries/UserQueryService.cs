using AutoMapper;
using Microsoft.Extensions.Logging;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Users;
using TravelAndAccommodationBookingPlatform.Application.Interfaces.Queries;
using TravelAndAccommodationBookingPlatform.Core.Exceptions;
using TravelAndAccommodationBookingPlatform.Core.Interfaces.Auth;
using TravelAndAccommodationBookingPlatform.Core.Interfaces.Repositories;

namespace TravelAndAccommodationBookingPlatform.Application.Services.Queries;

public class UserQueryService : IUserQueryService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IMapper _mapper;
    private readonly ILogger<UserQueryService> _logger;

    public UserQueryService(
        IUserRepository userRepository, 
        IMapper mapper,
        IJwtTokenGenerator jwtTokenGenerator, 
        ILogger<UserQueryService> logger)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _jwtTokenGenerator = jwtTokenGenerator;
        _logger = logger;
    }

    public async Task<TokenResponseDto> LogInAsync(LogInDto logInDto)
    {
        _logger.LogInformation("Login attempt started for email: {Email}", logInDto.Email);

        var authenticatedUser = await _userRepository.AuthenticateUserAsync(logInDto.Email, logInDto.Password);
        if (authenticatedUser is null)
        {
            _logger.LogWarning("Login failed: Invalid credentials for email '{Email}'.", logInDto.Email);
            throw new InvalidCredentialsException("Invalid email or password.");
        }
        
        var jwtToken = _jwtTokenGenerator.GenerateToken(authenticatedUser);

        _logger.LogInformation("Login successful for email: {Email}", logInDto.Email);

        return _mapper.Map<TokenResponseDto>(jwtToken);
    }
}
