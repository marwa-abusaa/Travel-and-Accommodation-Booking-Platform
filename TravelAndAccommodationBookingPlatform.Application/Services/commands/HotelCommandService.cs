using AutoMapper;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Hotels;
using TravelAndAccommodationBookingPlatform.Application.Interfaces.Commands;
using TravelAndAccommodationBookingPlatform.Core.Entities;
using TravelAndAccommodationBookingPlatform.Core.Exceptions;
using TravelAndAccommodationBookingPlatform.Core.Interfaces.Repositories;
using TravelAndAccommodationBookingPlatform.Core.Interfaces.UnitOfWork;

namespace TravelAndAccommodationBookingPlatform.Application.Services.Commands;

public class HotelCommandService : IHotelCommandService
{
    private readonly IHotelRepository _hotelRepository;
    private readonly ICityRepository _cityRepository;
    private readonly IOwnerRepository _ownerRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<HotelCommandService> _logger;
    private readonly IMapper _mapper;

    public HotelCommandService(
        IHotelRepository hotelRepository, 
        ICityRepository cityRepository, 
        IOwnerRepository ownerRepository, 
        IRoomRepository roomRepository, 
        IUnitOfWork unitOfWork, 
        ILogger<HotelCommandService> logger, 
        IMapper mapper)
    {
        _hotelRepository = hotelRepository;
        _cityRepository = cityRepository;
        _ownerRepository = ownerRepository;
        _roomRepository = roomRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<HotelResponseDto> AddHotelAsync(CreateHotelDto createHotelDto)
    {
        _logger.LogInformation("Attempting to add a new hotel at location: {Location}", createHotelDto.Location);

        if (!await _cityRepository.IsCityExistsAsync(c => c.CityId == createHotelDto.CityId))
        {
            _logger.LogWarning("City with ID {CityId} not found.", createHotelDto.CityId);
            throw new NotFoundException($"City with ID '{createHotelDto.CityId}' not found.");
        }

        if (!await _ownerRepository.IsOwnerExistsAsync(o => o.OwnerId == createHotelDto.OwnerId))
        {
            _logger.LogWarning("Owner with ID {OwnerId} not found.", createHotelDto.OwnerId);
            throw new NotFoundException($"Owner with ID '{createHotelDto.OwnerId}' not found.");
        }           

        if (createHotelDto.StarRating < 1 || createHotelDto.StarRating > 5)
        {
            _logger.LogWarning("Invalid star rating: {Rating}", createHotelDto.StarRating);
            throw new ValidationException("Star rating must be between 1 and 5.");
        }
            
        var hotel = _mapper.Map<Hotel>(createHotelDto);
        var addedHotel = await _hotelRepository.AddHotelAsync(hotel);

        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Hotel '{HotelName}' added successfully with ID {HotelId}.", addedHotel.Name, addedHotel.HotelId);

        return _mapper.Map<HotelResponseDto>(addedHotel);
    }

    public async Task DeleteHotelByIdAsync(int hotelId)
    {
        if (!await _hotelRepository.IsHotelExistsAsync(h => h.HotelId == hotelId))
        {
            _logger.LogWarning("Hotel with ID {HotelId} not found.", hotelId);
            throw new NotFoundException($"Hotel with ID '{hotelId}' not found.");
        }

        if (await _roomRepository.IsRoomExistsAsync(r=> r.HotelId == hotelId))
        {
            _logger.LogWarning("Cannot delete hotel {HotelId} because it has dependent rooms.", hotelId);
            throw new BadRequestException("Hotel cannot be deleted because it has assigned rooms.");
        }

        await _hotelRepository.DeleteHotelByIdAsync(hotelId);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Hotel with ID {HotelId} deleted successfully.", hotelId);
    }

    public async Task UpdateHotelAsync(UpdateHotelDto updateHotelDto)
    {
        var existingHotel = await _hotelRepository.GetHotelByIdAsync(updateHotelDto.HotelId);
        if (existingHotel is null)
        {
            _logger.LogWarning("Hotel with ID {HotelId} not found.", updateHotelDto.HotelId);
            throw new NotFoundException($"Hotel with ID '{updateHotelDto.HotelId}' not found.");
        }

        if (!await _cityRepository.IsCityExistsAsync(c => c.CityId == updateHotelDto.CityId))
        {
            _logger.LogWarning("City with ID {CityId} not found.", updateHotelDto.CityId);
            throw new NotFoundException($"City with ID '{updateHotelDto.CityId}' not found.");
        }

        if (!await _ownerRepository.IsOwnerExistsAsync(o => o.OwnerId == updateHotelDto.OwnerId))
        {
            _logger.LogWarning("Owner with ID {OwnerId} not found.", updateHotelDto.OwnerId);
            throw new NotFoundException($"Owner with ID '{updateHotelDto.OwnerId}' not found.");
        }      

        if (updateHotelDto.StarRating < 1 || updateHotelDto.StarRating > 5)
        {
            _logger.LogWarning("Invalid star rating: {Rating}", updateHotelDto.StarRating);
            throw new ValidationException("Star rating must be between 1 and 5.");
        }


        _mapper.Map(updateHotelDto, existingHotel);
        await _hotelRepository.UpdateHotelAsync(existingHotel);

        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Hotel with ID {HotelId} updated successfully.", updateHotelDto.HotelId);
    }
}
