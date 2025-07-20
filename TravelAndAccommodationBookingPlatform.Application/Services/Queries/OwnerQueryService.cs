using AutoMapper;
using Microsoft.Extensions.Logging;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Owners;
using TravelAndAccommodationBookingPlatform.Application.Interfaces.Queries;
using TravelAndAccommodationBookingPlatform.Core.Exceptions;
using TravelAndAccommodationBookingPlatform.Core.Interfaces.Repositories;

namespace TravelAndAccommodationBookingPlatform.Application.Services.Queries;

public class OwnerQueryService : IOwnerQueryService
{
    private readonly IOwnerRepository _ownerRepository;
    private readonly ILogger<OwnerQueryService> _logger;
    private readonly IMapper _mapper;

    public OwnerQueryService(
        IOwnerRepository ownerRepository, 
        ILogger<OwnerQueryService> logger, 
        IMapper mapper)
    {
        _ownerRepository = ownerRepository;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<OwnerResponseDto?> GetOwnerByHotelIdAsync(int hotelId)
    {
        _logger.LogInformation("Fetching owner for hotel ID {HotelId}.", hotelId);

        var owner = await _ownerRepository.GetOwnerByHotelIdAsync(hotelId);
        if (owner is null)
        {
            _logger.LogWarning("No owner found for hotel ID {HotelId}.", hotelId);
            throw new NotFoundException($"No owner found for hotel with ID '{hotelId}'.");
        }

        _logger.LogInformation("Owner for hotel ID {HotelId} retrieved successfully.", hotelId);

        return _mapper.Map<OwnerResponseDto>(owner);
    }

    public async Task<OwnerResponseDto?> GetOwnerByIdAsync(int ownerId)
    {
        var owner = await _ownerRepository.GetOwnerByIdAsync(ownerId);
        if (owner is null)
        {
            _logger.LogWarning("Owner with ID {OwnerId} not found.", ownerId);
            throw new NotFoundException($"Owner with ID '{ownerId}' not found.");
        }

        _logger.LogInformation("Owner with ID {OwnerId} retrieved successfully.", ownerId);

        return _mapper.Map<OwnerResponseDto>(owner);
    }
}
