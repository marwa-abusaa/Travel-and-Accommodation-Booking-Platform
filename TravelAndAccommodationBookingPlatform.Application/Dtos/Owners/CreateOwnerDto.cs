namespace TravelAndAccommodationBookingPlatform.Application.Dtos.Owners;

public class CreateOwnerDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
}
