using AutoMapper;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Bookings;
using TravelAndAccommodationBookingPlatform.Core.Entities;

namespace TravelAndAccommodationBookingPlatform.Application.Mapping;

public class BookingProfile : Profile
{
    public BookingProfile()
    {
        CreateMap<Booking, BookingResponseDto>();
    }
}
