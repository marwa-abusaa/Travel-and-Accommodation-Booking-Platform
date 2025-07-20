using Moq;
using AutoMapper;
using Microsoft.Extensions.Logging;
using TravelAndAccommodationBookingPlatform.Application.Services.Queries;
using TravelAndAccommodationBookingPlatform.Core.Entities;
using TravelAndAccommodationBookingPlatform.Core.Exceptions;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Users;
using TravelAndAccommodationBookingPlatform.Core.Interfaces.Auth;
using TravelAndAccommodationBookingPlatform.Core.Interfaces.Repositories;
using TravelAndAccommodationBookingPlatform.Core.Models;

namespace TravelAndAccommodationBookingPlatform.Tests.Queries;

public class UserQueryServiceTests
{
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<IJwtTokenGenerator> _jwtTokenGenMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<UserQueryService>> _loggerMock;
    private readonly UserQueryService _service;

    public UserQueryServiceTests()
    {
        _userRepoMock = new Mock<IUserRepository>();
        _jwtTokenGenMock = new Mock<IJwtTokenGenerator>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<UserQueryService>>();

        _service = new UserQueryService(
            _userRepoMock.Object,
            _mapperMock.Object,
            _jwtTokenGenMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task LogInAsync_ShouldReturnTokenResponse_WhenCredentialsAreValid()
    {
        // Arrange
        var loginDto = new LogInDto { Email = "test@example.com", Password = "password123" };
        var user = new User { Email = loginDto.Email };

        var jwtToken = new JwtToken("jwt_token_here");

        var tokenResponseDto = new TokenResponseDto { Token = jwtToken.token };

        _userRepoMock.Setup(x => x.AuthenticateUserAsync(loginDto.Email, loginDto.Password))
            .ReturnsAsync(user);

        _jwtTokenGenMock.Setup(x => x.GenerateToken(user))
            .Returns(jwtToken);

        _mapperMock.Setup(x => x.Map<TokenResponseDto>(jwtToken))
            .Returns(tokenResponseDto);

        // Act
        var result = await _service.LogInAsync(loginDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(tokenResponseDto.Token, result.Token);

        _userRepoMock.Verify(x => x.AuthenticateUserAsync(loginDto.Email, loginDto.Password), Times.Once);
        _jwtTokenGenMock.Verify(x => x.GenerateToken(user), Times.Once);
        _mapperMock.Verify(x => x.Map<TokenResponseDto>(jwtToken), Times.Once);
    }


    [Fact]
    public async Task LogInAsync_ShouldThrowInvalidCredentialsException_WhenUserIsNull()
    {
        // Arrange
        var loginDto = new LogInDto { Email = "wrong@example.com", Password = "wrongpass" };

        _userRepoMock.Setup(x => x.AuthenticateUserAsync(loginDto.Email, loginDto.Password))
            .ReturnsAsync((User)null);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidCredentialsException>(() => _service.LogInAsync(loginDto));
        Assert.Equal("Invalid email or password.", ex.Message);

        _userRepoMock.Verify(x => x.AuthenticateUserAsync(loginDto.Email, loginDto.Password), Times.Once);
        _jwtTokenGenMock.Verify(x => x.GenerateToken(It.IsAny<User>()), Times.Never);
        _mapperMock.Verify(x => x.Map<TokenResponseDto>(It.IsAny<TokenResponseDto>()), Times.Never);
    }
}
