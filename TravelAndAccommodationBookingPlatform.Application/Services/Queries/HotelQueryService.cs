using AutoMapper;
using Microsoft.Extensions.Logging;
using Sieve.Services;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Hotels;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Images;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Reviews;
using TravelAndAccommodationBookingPlatform.Application.Interfaces.Queries;
using TravelAndAccommodationBookingPlatform.Core.Exceptions;
using TravelAndAccommodationBookingPlatform.Core.Interfaces.Repositories;
using TravelAndAccommodationBookingPlatform.Core.Models;
using Microsoft.EntityFrameworkCore;
using Sieve.Models;

namespace TravelAndAccommodationBookingPlatform.Application.Services.Queries;

public class HotelQueryService : IHotelQueryService
{
    private readonly IHotelRepository _hotelRepository;
    private readonly IUserRepository _userRepository;
    private readonly IOwnerRepository _ownerRepository;
    private readonly ICityRepository _cityRepository;
    private readonly IReviewRepository _reviewRepository;
    private readonly IImageRepository _imageRepository;
    private readonly ILogger<HotelQueryService> _logger;
    private readonly ISieveProcessor _sieveProcessor;
    private readonly IMapper _mapper;

    public HotelQueryService(
        IHotelRepository hotelRepository,
        IUserRepository userRepository,
        IOwnerRepository ownerRepository,
        ICityRepository cityRepository,
        IReviewRepository reviewRepository,
        IImageRepository imageRepository,
        ILogger<HotelQueryService> logger,
        ISieveProcessor sieveProcessor,
        IMapper mapper
        )
    {
        _hotelRepository = hotelRepository;
        _userRepository = userRepository;
        _ownerRepository = ownerRepository;
        _cityRepository = cityRepository;
        _reviewRepository = reviewRepository;
        _imageRepository = imageRepository;        
        _sieveProcessor = sieveProcessor;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<IEnumerable<FeaturedHotelDto>> GetFeaturedDealsAsync(int count)
    {
        _logger.LogInformation("Retrieving top {Count} featured hotel deals...", count);

        var featuredHotels = await _hotelRepository.GetFeaturedDealsAsync(count);

        _logger.LogInformation("Successfully retrieved {Count} featured deals.", featuredHotels.Count());

        return featuredHotels.Select(h => new FeaturedHotelDto
        {
            HotelId = h.HotelId,
            HotelName = h.HotelName,
            Location = h.Location,
            Thumbnail = h.Thumbnail,
            StarRating = h.StarRating,
            OriginalPrice = h.OriginalPrice,
            DiscountedPrice = h.DiscountedPrice
        }).ToList();
    }

    public async Task<IEnumerable<RecentHotelDto>> GetRecentVisitedHotelsAsync(int userId)
    {
        _logger.LogInformation("Retrieving recently visited hotels for user ID: {UserId}", userId);

        var user = await _userRepository.GetUserByIdAsync(userId);
        if (user is null)
        {
            _logger.LogWarning("User with ID {UserId} not found.", userId);
            throw new NotFoundException($"User with ID '{userId}' not found.");
        }

        var hotels = await _hotelRepository.GetRecentVisitedHotelsByUserIdAsync(userId);

        _logger.LogInformation("Successfully fetched recently visited hotels for user ID: {UserId}", userId);
       
        return _mapper.Map<IEnumerable<RecentHotelDto>>(hotels);
    }

    public async Task<HotelDetailsDto?> GetHotelDetailsAsync(int hotelId)
    {
        _logger.LogInformation("Retrieving hotel details for hotel ID: {HotelId}", hotelId);

        var hotel = await _hotelRepository.GetHotelByIdAsync(hotelId);

        if (hotel is null)
        {
            _logger.LogWarning("Hotel with ID {HotelId} not found.", hotelId);
            throw new NotFoundException($"Hotel with ID '{hotelId}' not found.");
        }

        var reviewRating = await _reviewRepository.GetHotelRatingAsync(hotelId);
        var hotelImages = await _imageRepository.GetHotelImagesAsync(hotelId);

        var hotelDetails = new HotelDetailsDto
        {
            Name = hotel.Name,
            Location = hotel.Location,
            StarRating = hotel.StarRating,
            ReviewRating = reviewRating,
            FullDescription = hotel.FullDescription,
            CityName = hotel.City.Name,
            OwnerName = hotel.Owner.FirstName,
            PhoneNumber = hotel.PhoneNumber,
        };

        hotelDetails.Images= _mapper.Map<List<ImageResponseDto>>(hotelImages);
        hotelDetails.Reviews = _mapper.Map<List<ReviewResponseDto>>(hotel.Reviews);

        _logger.LogInformation("Hotel details for ID {HotelId} retrieved successfully.", hotelId);

        return hotelDetails;
    }

    public async Task<IEnumerable<HotelSearchDto>> GetHotelsByOwnerIdAsync(int ownerId)
    {
        _logger.LogInformation("Retrieving hotels for owner ID: {OwnerId}", ownerId);

        if (!await _ownerRepository.IsOwnerExistsAsync(o => o.OwnerId == ownerId))
        {
            _logger.LogWarning("Owner with ID {OwnerId} not found.", ownerId);
            throw new NotFoundException($"Owner with ID '{ownerId}' not found.");
        }

        var hotels = await _hotelRepository.GetHotelsByOwnerIdAsync(ownerId);

        var ownerHotelDtos = hotels.Select(h => new HotelSearchDto
        {
            HotelId = h.HotelId,
            Name = h.Name,
            Location = h.Location,
            StarRating = h.StarRating,
            Thumbnail = _mapper.Map<ImageResponseDto>(h.Thumbnail),
            PricePerNight = h.Rooms.Any()
            ? (double)h.Rooms.Min(r => r.PricePerNight)
            : 0
        });

        _logger.LogInformation("Successfully retrieved {Count} hotels for owner ID {OwnerId}.", hotels.Count(), ownerId);

        return ownerHotelDtos;      
    }

    public async Task<PaginatedResult<HotelSearchDto>> GetHotelsByCityIdAsync(int cityId, PaginationMetadata pagination)
    {
        _logger.LogInformation("Retrieving hotels for city with ID: {CityId}", cityId);

        if (!await _cityRepository.IsCityExistsAsync(c => c.CityId == cityId))
        {
            _logger.LogWarning("City with ID {CityId} not found.", cityId);
            throw new NotFoundException($"City with ID '{cityId}' not found.");
        }

        var hotels = await _hotelRepository.GetHotelsByCityIdAsync(cityId, pagination);

        var cityHotelDtos = hotels.Items.Select(h => new HotelSearchDto
        {
            HotelId = h.HotelId,
            Name = h.Name,
            Location = h.Location,
            StarRating = h.StarRating,
            Thumbnail = _mapper.Map<ImageResponseDto>(h.Thumbnail),
            PricePerNight = h.Rooms.Any()
            ? (double)h.Rooms.Min(r => r.PricePerNight)
            : 0
        }).ToList();

        _logger.LogInformation("Retrieved {Count} hotels for city with cityID: {CityId}.", cityHotelDtos.Count, cityId);

        return new PaginatedResult<HotelSearchDto>(cityHotelDtos, hotels.PaginationMetadata!);
    }

    public async Task<PaginatedResult<HotelSearchDto>> SearchHotelsAsync(HotelSearchRequest request)
    {
        _logger.LogInformation("Searching hotels with filters: CheckIn={CheckIn}, CheckOut={CheckOut}, Adults={Adults}, Children={Children}, Rooms={Rooms}",
           request.CheckIn, request.CheckOut, request.Adults, request.Children, request.Rooms);

        var query = _hotelRepository.GetAllAsQueryable();

        if (request.CheckIn.HasValue && request.CheckOut.HasValue)
        {
            var checkIn = request.CheckIn;
            var checkOut = request.CheckOut;
            var adults = request.Adults;
            var children = request.Children;

            query = query.Where(h =>
                h.Rooms.Where(r =>
                    r.AdultCapacity >= adults &&
                    r.ChildrenCapacity >= children &&
                    r.Bookings.All(b =>
                        b.CheckOutDate <= checkIn || b.CheckInDate >= checkOut
                    )
                ).Count() >= request.Rooms
            );
        }

        var filtered = _sieveProcessor.Apply(request, query, applyPagination: false);
        var totalCount = await filtered.CountAsync();

        var pagedQuery = _sieveProcessor.Apply(request, query);
        var data = await _mapper.ProjectTo<HotelSearchDto>(pagedQuery).ToListAsync();

        var pagination = new PaginationMetadata
        {
            PageNumber = request.Page ?? 1,
            PageSize = request.PageSize ?? 10,
            TotalCount = totalCount
        };

        _logger.LogInformation("Hotel search completed. Total results: {Total}", totalCount);

        return new PaginatedResult<HotelSearchDto>(data, pagination);
    }


    public async Task<PaginatedResult<AdminHotelResponseDto>> SearchHotelsAdminAsync(SieveModel request)
    {
        _logger.LogInformation("Admin hotel search initiated. Filters: Page={Page}, PageSize={PageSize}",
           request.Page, request.PageSize);

        var query = _hotelRepository.GetAllAsQueryable()
          .Select(h => new AdminHotelResponseDto
          {
              HotelId = h.HotelId,
              Name = h.Name,
              StarRating = h.StarRating,
              OwnerName = h.Owner.FirstName + " " + h.Owner.LastName,
              RoomCount = h.Rooms.Count,
              CreatedAt = h.CreatedAt,
              UpdatedAt = h.UpdatedAt
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

        _logger.LogInformation("Admin hotel search completed. Total matches: {Total}", totalCount);

        return new PaginatedResult<AdminHotelResponseDto>(data, pagination);
    }   
}
