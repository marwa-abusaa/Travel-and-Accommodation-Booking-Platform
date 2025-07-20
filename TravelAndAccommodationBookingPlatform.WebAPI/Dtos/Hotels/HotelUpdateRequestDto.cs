namespace TravelAndAccommodationBookingPlatform.WebAPI.Dtos.Hotels;

public class HotelUpdateRequestDto
{
    public string Name { get; set; }
    public int CityId { get; set; }
    public int OwnerId { get; set; }
    public string FullDescription { get; set; }
    public string BriefDescription { get; set; }
    public string PhoneNumber { get; set; }
    public int StarRating { get; set; }
    public string Location { get; set; }
}
