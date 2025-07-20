using AutoMapper;
using Microsoft.Extensions.Logging;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Owners;
using TravelAndAccommodationBookingPlatform.Application.Interfaces.Commands;
using TravelAndAccommodationBookingPlatform.Core.Entities;
using TravelAndAccommodationBookingPlatform.Core.Exceptions;
using TravelAndAccommodationBookingPlatform.Core.Interfaces.Repositories;
using TravelAndAccommodationBookingPlatform.Core.Interfaces.UnitOfWork;

namespace TravelAndAccommodationBookingPlatform.Application.Services.Commands;

public class OwnerCommandService : IOwnerCommandService
{
    private readonly IOwnerRepository _ownerRepository;
    private readonly ILogger<OwnerCommandService> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public OwnerCommandService(
        IOwnerRepository ownerRepository, 
        ILogger<OwnerCommandService> logger, 
        IUnitOfWork unitOfWork, 
        IMapper mapper)
    {
        _ownerRepository = ownerRepository;
        _logger = logger;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<OwnerResponseDto> AddOwnerAsync(CreateOwnerDto createOwnerDto)
    {
        if(await _ownerRepository.IsOwnerExistsAsync(o => o.Email == createOwnerDto.Email))
        {
            _logger.LogWarning("Email {Email} already exists.", createOwnerDto.Email);
            throw new EmailAlreadyExistsException($"Email '{createOwnerDto.Email}' is already in use.");
        }

        var owner = _mapper.Map<Owner>(createOwnerDto);
        var addedOwner = await _ownerRepository.AddOwnerAsync(owner);

        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Owner with ID {OwnerId} added successfully.", addedOwner.OwnerId);

        return _mapper.Map<OwnerResponseDto>(addedOwner);
    }

    public async Task UpdateOwnerAsync(UpdateOwnerDto updateOwnerDto)
    {
        var existingOwner = await _ownerRepository.GetOwnerByIdAsync(updateOwnerDto.OwnerId);
        if (existingOwner is null)
        {
            _logger.LogWarning("Owner with ID {OwnerId} not found.", updateOwnerDto.OwnerId);
            throw new NotFoundException($"Owner with ID '{updateOwnerDto.OwnerId}' not found.");
        }

        _mapper.Map(updateOwnerDto, existingOwner);
        await _ownerRepository.UpdateOwnerAsync(existingOwner);

        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Owner with ID {OwnerId} updated successfully.", updateOwnerDto.OwnerId);
    }
}
