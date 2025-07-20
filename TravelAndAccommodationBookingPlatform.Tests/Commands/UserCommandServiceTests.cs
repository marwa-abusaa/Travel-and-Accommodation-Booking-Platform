using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using FluentAssertions;
using TravelAndAccommodationBookingPlatform.Application.Services.Commands;
using TravelAndAccommodationBookingPlatform.Core.Entities;
using TravelAndAccommodationBookingPlatform.Core.Exceptions;
using TravelAndAccommodationBookingPlatform.Core.Interfaces.Repositories;
using TravelAndAccommodationBookingPlatform.Core.Interfaces.UnitOfWork;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Users;

namespace TravelAndAccommodationBookingPlatform.Tests.Commands;

public class UserCommandServiceTests
{
    private readonly Mock<IUserRepository> _userRepoMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IRoleRepository> _roleRepoMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<ILogger<UserCommandService>> _loggerMock = new();

    private readonly UserCommandService _service;

    public UserCommandServiceTests()
    {
        _service = new UserCommandService(
            _userRepoMock.Object,
            _mapperMock.Object,
            _roleRepoMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }


    [Fact]
    public async Task SignUpAsync_ShouldAddUserAndSave_WhenDataIsValid()
    {
        // Arrange
        var signUpDto = new SignUpDto
        {
            Email = "test@example.com",
            RoleName = "User",
            FirstName = "John",
            LastName = "Doe",
            Password = "pass123",
            PhoneNumber = "123456789",
            DateOfBirth = DateTime.UtcNow.AddYears(-20)
        };

        var role = new Role { RoleId = 1, Name = "User" };
        var userEntity = new User { Email = signUpDto.Email, Role = role };

        _userRepoMock.Setup(u => u.IsEmailExistsAsync(signUpDto.Email)).ReturnsAsync(false);
        _roleRepoMock.Setup(r => r.GetRoleByNameAsync(signUpDto.RoleName)).ReturnsAsync(role);
        _mapperMock.Setup(m => m.Map<User>(signUpDto)).Returns(userEntity);
        _userRepoMock.Setup(u => u.AddUserAsync(userEntity)).Returns(Task.CompletedTask);

        // Act
        await _service.SignUpAsync(signUpDto);

        // Assert
        _userRepoMock.Verify(u => u.AddUserAsync(userEntity), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task SignUpAsync_ShouldThrowEmailAlreadyExistsException_WhenEmailExists()
    {
        // Arrange
        var signUpDto = new SignUpDto { Email = "test@example.com" };
        _userRepoMock.Setup(u => u.IsEmailExistsAsync(signUpDto.Email)).ReturnsAsync(true);

        // Act
        Func<Task> act = () => _service.SignUpAsync(signUpDto);

        // Assert
        await act.Should().ThrowAsync<EmailAlreadyExistsException>()
            .WithMessage("Email is already registered.");
    }

    [Fact]
    public async Task SignUpAsync_ShouldThrowNotFoundException_WhenRoleNotFound()
    {
        // Arrange
        var signUpDto = new SignUpDto { Email = "test@example.com", RoleName = "Role" };
        _userRepoMock.Setup(u => u.IsEmailExistsAsync(signUpDto.Email)).ReturnsAsync(false);
        _roleRepoMock.Setup(r => r.GetRoleByNameAsync(signUpDto.RoleName)).ReturnsAsync((Role?)null);

        // Act
        Func<Task> act = () => _service.SignUpAsync(signUpDto);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Role '{signUpDto.RoleName}' not found.");
    }

}
