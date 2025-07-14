using AutoMapper;
using Microsoft.Extensions.Logging;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Discounts;
using TravelAndAccommodationBookingPlatform.Application.Interfaces.Queries;
using TravelAndAccommodationBookingPlatform.Core.Exceptions;
using TravelAndAccommodationBookingPlatform.Core.Interfaces.Repositories;
using TravelAndAccommodationBookingPlatform.Core.Models;

namespace TravelAndAccommodationBookingPlatform.Application.Services.Queries;

public class DiscountQueryService : IDiscountQueryService
{
    private readonly IDiscountRepository _discountRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly ILogger<DiscountQueryService> _logger;
    private readonly IMapper _mapper;

    public DiscountQueryService(
        IDiscountRepository discountRepository,
        IRoomRepository roomRepository, 
        ILogger<DiscountQueryService> logger, 
        IMapper mapper)
    {
        _discountRepository = discountRepository;
        _roomRepository = roomRepository;
        _logger = logger;
        _mapper = mapper;
    }


    public async Task<DiscountResponseDto?> GetDiscountByIdAsync(int discountId)
    {
        var discount = await _discountRepository.GetDiscountByIdAsync(discountId);
        if (discount is null)
        {
            _logger.LogWarning("Discount with ID {DiscountId} was not found.", discountId);
            throw new NotFoundException($"Discount with ID '{discountId}' not found.");
        }

        _logger.LogInformation("Successfully retrieved discount with ID: {DiscountId}", discountId);

        return _mapper.Map<DiscountResponseDto>(discount);
    }

    public async Task<IEnumerable<DiscountResponseDto>> GetDiscountByRoomIdAsync(int roomId)
    {
        _logger.LogInformation("Fetching discounts for room ID: {RoomId}", roomId);

        var room = await _roomRepository.GetRoomByIdAsync(roomId);
        if (room is null)
        {
            _logger.LogWarning("Room with ID {RoomId} not found when fetching discounts.", roomId);
            throw new NotFoundException($"Room with ID '{roomId}' not found.");
        }

        var discounts = await _discountRepository.GetDiscountByRoomIdAsync(roomId);

        _logger.LogInformation("Retrieved {Count} discounts for room ID: {RoomId}", discounts.Count(), roomId);

        return _mapper.Map<List<DiscountResponseDto>>(discounts);
    }

    public async Task<PaginatedResult<DiscountResponseDto>> GetDiscountsAsync(PaginationMetadata pagination)
    {
        _logger.LogInformation("Fetching all discounts");

        var discounts = await _discountRepository.GetDiscountsAsync(pagination);

        _logger.LogInformation("Retrieved {Count} discounts.", discounts.Items.Count);

        return _mapper.Map<PaginatedResult<DiscountResponseDto>>(discounts);
    }
}
