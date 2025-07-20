using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Sieve.Services;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Cities;
using TravelAndAccommodationBookingPlatform.Application.Services.Queries;
using TravelAndAccommodationBookingPlatform.Core.Entities;
using TravelAndAccommodationBookingPlatform.Core.Exceptions;
using TravelAndAccommodationBookingPlatform.Core.Interfaces.Repositories;

namespace TravelAndAccommodationBookingPlatform.Tests.Queries;

public class CityQueryServiceTests
{
    private readonly Mock<ICityRepository> _cityRepositoryMock = new();
    private readonly Mock<ISieveProcessor> _sieveProcessorMock = new();
    private readonly IMapper _mapper;
    private readonly CityQueryService _service;
    private readonly Mock<ILogger<CityQueryService>> _loggerMock = new();

    public CityQueryServiceTests()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<City, CityResponseDto>();
            cfg.CreateMap<City, AdminCityResponseDto>();
        });
        _mapper = config.CreateMapper();

        _service = new CityQueryService(
            _cityRepositoryMock.Object,
            _loggerMock.Object,
            _sieveProcessorMock.Object,
            _mapper);
    }

    [Fact]
    public async Task GetCityByIdAsync_ShouldReturnCity_WhenCityExists()
    {
        // Arrange
        int cityId = 1;
        var city = new City
        {
            CityId = cityId,
            Name = "Test City",
            Country = "TestCountry",
            PostOffice = "12345"
        };

        _cityRepositoryMock.Setup(repo => repo.GetCityByIdAsync(cityId))
            .ReturnsAsync(city);

        // Act
        var result = await _service.GetCityByIdAsync(cityId);

        // Assert
        result.Should().NotBeNull();
        result.CityId.Should().Be(cityId);
        result.Name.Should().Be(city.Name);
    }

    [Fact]
    public async Task GetCityByIdAsync_ShouldThrowNotFoundException_WhenCityDoesNotExist()
    {
        // Arrange
        int cityId = 99;
        _cityRepositoryMock.Setup(repo => repo.GetCityByIdAsync(cityId))
            .ReturnsAsync((City?)null);

        // Act
        Func<Task> act = () => _service.GetCityByIdAsync(cityId);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"City with ID '{cityId}' was not found.");
    }

    [Fact]
    public async Task GetMostVisitedCitiesAsync_ShouldReturnCities_WhenCountIsValid()
    {
        // Arrange
        int count = 2;
        var cities = new List<City>
        {
            new City { CityId = 1, Name = "City1" },
            new City { CityId = 2, Name = "City2" }
        }.AsQueryable();

        _cityRepositoryMock.Setup(repo => repo.GetMostVisitedCitiesAsync(count))
            .ReturnsAsync(cities.ToList());

        // Act
        var result = await _service.GetMostVisitedCitiesAsync(count);

        // Assert
        result.Should().HaveCount(count);
        result.Select(c => c.Name).Should().Contain(new[] { "City1", "City2" });
    }

    [Fact]
    public async Task GetMostVisitedCitiesAsync_ShouldThrowArgumentException_WhenCountIsInvalid()
    {
        // Arrange
        int count = 0;

        // Act
        Func<Task> act = () => _service.GetMostVisitedCitiesAsync(count);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("Count must be greater than zero.*");
    }

}
