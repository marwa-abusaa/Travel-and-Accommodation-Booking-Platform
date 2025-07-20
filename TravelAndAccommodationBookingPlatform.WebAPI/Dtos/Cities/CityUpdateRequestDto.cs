namespace TravelAndAccommodationBookingPlatform.WebAPI.Dtos.Cities;

public class CityUpdateRequestDto
{
    public string Name { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string PostOffice { get; set; } = string.Empty;
}
