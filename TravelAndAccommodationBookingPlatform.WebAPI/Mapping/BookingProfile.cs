using AutoMapper;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Bookings;
using TravelAndAccommodationBookingPlatform.WebAPI.Dtos.Bookings;

namespace TravelAndAccommodationBookingPlatform.WebAPI.Mapping;

public class BookingProfile : Profile
{
    public BookingProfile()
    {
        CreateMap<BookingCreationRequestDto, CreateBookingDto>();
    }
}
