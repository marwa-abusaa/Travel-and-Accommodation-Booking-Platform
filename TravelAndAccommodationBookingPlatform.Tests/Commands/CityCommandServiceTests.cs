using AutoMapper;
using FluentAssertions;
using Moq;
using System.Linq.Expressions;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Cities;
using TravelAndAccommodationBookingPlatform.Application.Services.Commands;
using TravelAndAccommodationBookingPlatform.Core.Entities;
using TravelAndAccommodationBookingPlatform.Core.Exceptions;
using TravelAndAccommodationBookingPlatform.Core.Interfaces.Repositories;
using TravelAndAccommodationBookingPlatform.Core.Interfaces.UnitOfWork;

namespace TravelAndAccommodationBookingPlatform.Tests.Commands;

public class CityCommandServiceTests
{
    private readonly Mock<ICityRepository> _cityRepositoryMock = new();
    private readonly Mock<IHotelRepository> _hotelRepositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly IMapper _mapper;
    private readonly CityCommandService _service;

    public CityCommandServiceTests()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<CreateCityDto, City>();
            cfg.CreateMap<City, AdminCityResponseDto>();
            cfg.CreateMap<UpdateCityDto, City>();
        });
        _mapper = config.CreateMapper();

        _service = new CityCommandService(
            _cityRepositoryMock.Object,
            _hotelRepositoryMock.Object,
            _unitOfWorkMock.Object,
            Mock.Of<Microsoft.Extensions.Logging.ILogger<CityCommandService>>(),
            _mapper
        );
    }

    [Fact]
    public async Task AddCityAsync_ShouldAddCity_WhenNoCityHasSamePostOffice()
    {
        // Arrange
        var createDto = new CreateCityDto { PostOffice = "12345", Name = "New City", Country = "TestCountry" };
        var cityEntity = new City
        {
            CityId = 1,
            PostOffice = createDto.PostOffice,
            Name = createDto.Name,
            Country = createDto.Country
        };

        _cityRepositoryMock.Setup(repo => repo.IsCityExistsAsync(It.IsAny<Expression<Func<City, bool>>>()))
            .ReturnsAsync(false);

        _cityRepositoryMock.Setup(repo => repo.AddCityAsync(It.IsAny<City>()))
            .ReturnsAsync(cityEntity);

        // Act
        var result = await _service.AddCityAsync(createDto);

        // Assert
        result.Should().NotBeNull();
        result.CityId.Should().Be(cityEntity.CityId);
        result.PostOffice.Should().Be(createDto.PostOffice);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task AddCityAsync_ShouldThrowConflictException_WhenCityExists()
    {
        // Arrange
        var createDto = new CreateCityDto { PostOffice = "12345" };

        _cityRepositoryMock.Setup(repo => repo.IsCityExistsAsync(It.IsAny<Expression<Func<City, bool>>>()))
            .ReturnsAsync(true);

        // Act
        Func<Task> act = () => _service.AddCityAsync(createDto);

        // Assert
        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("*already exists*");

        _cityRepositoryMock.Verify(repo => repo.AddCityAsync(It.IsAny<City>()), Times.Never);
    }

    [Fact]
    public async Task DeleteCityByIdAsync_ShouldDeleteCity_WhenExistsAndNoHotels()
    {
        // Arrange
        int cityId = 1;

        _cityRepositoryMock.Setup(r => r.IsCityExistsAsync(It.IsAny<Expression<Func<City, bool>>>()))
            .ReturnsAsync(true);

        _hotelRepositoryMock.Setup(r => r.IsHotelExistsAsync(It.IsAny<Expression<Func<Hotel, bool>>>()))
            .ReturnsAsync(false);

        // Act
        await _service.DeleteCityByIdAsync(cityId);

        // Assert
        _cityRepositoryMock.Verify(r => r.DeleteCityByIdAsync(cityId), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteCityByIdAsync_ShouldThrowNotFoundException_WhenCityNotExists()
    {
        // Arrange
        int cityId = 999;

        _cityRepositoryMock.Setup(r => r.IsCityExistsAsync(It.IsAny<Expression<Func<City, bool>>>()))
            .ReturnsAsync(false);

        // Act
        Func<Task> act = () => _service.DeleteCityByIdAsync(cityId);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("*not found*");

        _cityRepositoryMock.Verify(r => r.DeleteCityByIdAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task DeleteCityByIdAsync_ShouldThrowBadRequestException_WhenCityHasHotels()
    {
        // Arrange
        int cityId = 2;

        _cityRepositoryMock.Setup(r => r.IsCityExistsAsync(It.IsAny<Expression<Func<City, bool>>>()))
            .ReturnsAsync(true);

        _hotelRepositoryMock.Setup(r => r.IsHotelExistsAsync(It.IsAny<Expression<Func<Hotel, bool>>>()))
            .ReturnsAsync(true);

        // Act
        Func<Task> act = () => _service.DeleteCityByIdAsync(cityId);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>()
            .WithMessage("*associated hotels*");

        _cityRepositoryMock.Verify(r => r.DeleteCityByIdAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task UpdateCityAsync_ShouldUpdateCity_WhenCityExists()
    {
        // Arrange
        var updateDto = new UpdateCityDto
        {
            CityId = 1,
            Name = "Updated City",
            Country = "CountryX",
            PostOffice = "99999"
        };

        var existingCity = new City
        {
            CityId = updateDto.CityId,
            Name = "Old City",
            Country = "OldCountry",
            PostOffice = "11111"
        };

        _cityRepositoryMock.Setup(r => r.GetCityByIdAsync(updateDto.CityId))
            .ReturnsAsync(existingCity);

        // Act
        await _service.UpdateCityAsync(updateDto);

        // Assert
        existingCity.Name.Should().Be(updateDto.Name);
        existingCity.Country.Should().Be(updateDto.Country);
        existingCity.PostOffice.Should().Be(updateDto.PostOffice);

        _cityRepositoryMock.Verify(r => r.UpdateCityAsync(existingCity), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateCityAsync_ShouldThrowNotFoundException_WhenCityNotExists()
    {
        // Arrange
        var updateDto = new UpdateCityDto { CityId = 999, Name = "NonExisting" };

        _cityRepositoryMock.Setup(r => r.GetCityByIdAsync(updateDto.CityId))
            .ReturnsAsync((City?)null);

        // Act
        Func<Task> act = () => _service.UpdateCityAsync(updateDto);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("*not found*");

        _cityRepositoryMock.Verify(r => r.UpdateCityAsync(It.IsAny<City>()), Times.Never);
    }
}

