using AutoMapper;
using Microsoft.Extensions.Logging;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Cities;
using TravelAndAccommodationBookingPlatform.Application.Interfaces.Commands;
using TravelAndAccommodationBookingPlatform.Core.Entities;
using TravelAndAccommodationBookingPlatform.Core.Exceptions;
using TravelAndAccommodationBookingPlatform.Core.Interfaces.Repositories;
using TravelAndAccommodationBookingPlatform.Core.Interfaces.UnitOfWork;

namespace TravelAndAccommodationBookingPlatform.Application.Services.Commands;

public class CityCommandService : ICityCommandService
{
    private readonly ICityRepository _cityRepository;
    private readonly IHotelRepository _hotelRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CityCommandService> _logger;
    private readonly IMapper _mapper;

    public CityCommandService(
        ICityRepository cityRepository,
        IHotelRepository hotelRepository,
        IUnitOfWork unitOfWork, 
        ILogger<CityCommandService> logger,
        IMapper mapper)
    {
        _cityRepository = cityRepository;
        _hotelRepository = hotelRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<AdminCityResponseDto> AddCityAsync(CreateCityDto createCityDto)
    {
        if (await _cityRepository.IsCityExistsAsync(c => c.PostOffice == createCityDto.PostOffice))
        {
            _logger.LogWarning("City with PostOffice '{PostOffice}' already exists.", createCityDto.PostOffice);
            throw new ConflictException($"City with PostOffice '{createCityDto.PostOffice}' already exists.");
        }

        var city = _mapper.Map<City>(createCityDto);
        var addedCity = await _cityRepository.AddCityAsync(city);

        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("City added successfully with ID: {CityId}", addedCity.CityId);

        return _mapper.Map<AdminCityResponseDto>(addedCity);
    }

    public async Task DeleteCityByIdAsync(int cityId)
    {
        if (!await _cityRepository.IsCityExistsAsync(c => c.CityId == cityId))
        {
            _logger.LogWarning("City with ID '{CityId}' not found.", cityId);
            throw new NotFoundException($"City with ID '{cityId}' not found.");
        }
        
        if(await _hotelRepository.IsHotelExistsAsync(h => h.CityId == cityId))
        {
            _logger.LogWarning("City with ID '{CityId}' has associated hotels and cannot be deleted.", cityId);
            throw new BadRequestException("Cannot delete city because it has associated hotels.");
        }

        await _cityRepository.DeleteCityByIdAsync(cityId);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("City with ID {CityId} deleted successfully.", cityId);
    }

    public async Task UpdateCityAsync(UpdateCityDto updateCityDto)
    {       
        var existingCity = await _cityRepository.GetCityByIdAsync(updateCityDto.CityId);
        if (existingCity is null)
        {
            _logger.LogWarning("City with ID '{CityId}' not found.", updateCityDto.CityId);
            throw new NotFoundException($"City with ID '{updateCityDto.CityId}' not found.");
        }

        _mapper.Map(updateCityDto, existingCity);
        await _cityRepository.UpdateCityAsync(existingCity);

        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("City with ID {CityId} updated successfully.", updateCityDto.CityId);
    }
}
