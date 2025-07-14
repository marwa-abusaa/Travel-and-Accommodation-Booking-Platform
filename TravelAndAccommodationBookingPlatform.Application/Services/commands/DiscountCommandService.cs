using AutoMapper;
using Microsoft.Extensions.Logging;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Discounts;
using TravelAndAccommodationBookingPlatform.Application.Interfaces.Commands;
using TravelAndAccommodationBookingPlatform.Core.Entities;
using TravelAndAccommodationBookingPlatform.Core.Exceptions;
using TravelAndAccommodationBookingPlatform.Core.Interfaces.Repositories;
using TravelAndAccommodationBookingPlatform.Core.Interfaces.UnitOfWork;

namespace TravelAndAccommodationBookingPlatform.Application.Services.Commands;

public class DiscountCommandService : IDiscountCommandService
{
    private readonly IDiscountRepository _discountRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DiscountCommandService> _logger;
    private readonly IMapper _mapper;

    public DiscountCommandService(
        IDiscountRepository discountRepository,
        IRoomRepository roomRepository, 
        IUnitOfWork unitOfWork, 
        ILogger<DiscountCommandService> logger,
        IMapper mapper)
    {
        _discountRepository = discountRepository;
        _roomRepository = roomRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<DiscountResponseDto> AddDiscountAsync(CreateDiscountDto createDiscountDto)
    {
        _logger.LogInformation("Attempting to add a discount to Room ID: {RoomId}", createDiscountDto.RoomId);

        var room = await _roomRepository.GetRoomByIdAsync(createDiscountDto.RoomId);
        if (room is null)
        {
            _logger.LogWarning("Room with ID {RoomId} not found. Cannot apply discount.", createDiscountDto.RoomId);
            throw new NotFoundException($"Room with ID '{createDiscountDto.RoomId}' not found.");
        }

        var discount = _mapper.Map<Discount>(createDiscountDto);
        var addedDiscount = await _discountRepository.AddDiscountAsync(discount);

        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Discount added successfully for Room ID: {RoomId}, Discount ID: {DiscountId}",
        addedDiscount.RoomId, addedDiscount.DiscountId);

        return _mapper.Map<DiscountResponseDto>(addedDiscount);
    }

    public async Task DeleteDiscountByIdAsync(int discountId)
    {
        var discount = await _discountRepository.GetDiscountByIdAsync(discountId);
        if (discount is null)
        {
            _logger.LogWarning("Discount with ID {DiscountId} not found. Delete operation aborted.", discountId);
            throw new NotFoundException($"Discount with ID '{discountId}' not found.");
        }

        await _discountRepository.DeleteDiscountByIdAsync(discountId);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Discount with ID {DiscountId} deleted successfully.", discountId);
    }
}
