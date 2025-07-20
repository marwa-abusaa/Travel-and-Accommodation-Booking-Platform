using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sieve.Models;
using Sieve.Services;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Cities;
using TravelAndAccommodationBookingPlatform.Application.Interfaces.Queries;
using TravelAndAccommodationBookingPlatform.Core.Exceptions;
using TravelAndAccommodationBookingPlatform.Core.Interfaces.Repositories;
using TravelAndAccommodationBookingPlatform.Core.Models;

namespace TravelAndAccommodationBookingPlatform.Application.Services.Queries;

public class CityQueryService : ICityQueryService
{
    private readonly ICityRepository _cityRepository;
    private readonly ILogger<CityQueryService> _logger;
    private readonly ISieveProcessor _sieveProcessor;
    private readonly IMapper _mapper;

    public CityQueryService(
        ICityRepository cityRepository,
        ILogger<CityQueryService> logger,
        ISieveProcessor sieveProcessor,
        IMapper mapper
        )
    {
        _cityRepository = cityRepository;
        _logger = logger;
        _sieveProcessor = sieveProcessor;
        _mapper = mapper;
    }


    public async Task<CityResponseDto?> GetCityByIdAsync(int cityId)
    {
        var city = await _cityRepository.GetCityByIdAsync(cityId);
        if (city is null)
        {
            _logger.LogWarning("City with ID {CityId} was not found.", cityId);
            throw new NotFoundException($"City with ID '{cityId}' was not found.");
        }

        _logger.LogInformation("Successfully retrieved city with ID: {CityId}", cityId);

        return _mapper.Map<CityResponseDto>(city);
    }

    public async Task<IEnumerable<CityResponseDto>> GetMostVisitedCitiesAsync(int count)
    {
        _logger.LogInformation("Fetching top {Count} most visited cities.", count);

        if (count <= 0)
        {
            _logger.LogWarning("Invalid count value: {Count}. It must be greater than zero.", count);
            throw new ArgumentException("Count must be greater than zero.", nameof(count));
        }
            
        var mostCities = await _cityRepository.GetMostVisitedCitiesAsync(count);

        _logger.LogInformation("Retrieved {Count} most visited cities.", mostCities.Count());

        return _mapper.Map<IEnumerable<CityResponseDto>>(mostCities);
    }

    public async Task<PaginatedResult<AdminCityResponseDto>> SearchCitiesAsync(SieveModel request)
    {
        var query = _cityRepository.GetAllAsQueryable()
            .Select(c => new AdminCityResponseDto
            {
                CityId=c.CityId,
                Name = c.Name,
                Country = c.Country,
                PostOffice = c.PostOffice,
                HotelsCount = c.Hotels.Count,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            });


        var filtered = _sieveProcessor.Apply(request, query, applyPagination: false);
        var totalCount = await filtered.CountAsync();

        var pagedQuery = _sieveProcessor.Apply(request, query);

        var pagination = new PaginationMetadata
        {
            PageNumber = request.Page ?? 1,
            PageSize = request.PageSize ?? 10,
            TotalCount = totalCount
        };

        var data = await pagedQuery.ToListAsync();

        _logger.LogInformation($"Successfully retrieved Cities search result");

        return new PaginatedResult<AdminCityResponseDto>(data, pagination);
    }
}
