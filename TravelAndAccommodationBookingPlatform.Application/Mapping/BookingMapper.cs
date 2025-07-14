using AutoMapper;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Bookings;
using TravelAndAccommodationBookingPlatform.Core.Entities;

namespace TravelAndAccommodationBookingPlatform.Application.Mapping;

public class BookingMapper : Profile
{
    public BookingMapper()
    {
        CreateMap<CreateBookingDto, Booking>();
        CreateMap<Booking, BookingResponseDto>();
    }
}
