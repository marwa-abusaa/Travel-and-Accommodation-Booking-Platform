namespace TravelAndAccommodationBookingPlatform.Application.Dtos.Cities;

public class UpdateCityDto
{
    public int CityId { get; set; }
    public string Name { get; set; }
    public string Country { get; set; }
    public string PostOffice { get; set; }
}
