namespace TravelAndAccommodationBookingPlatform.Application.Dtos.Owners;

public class UpdateOwnerDto
{
    public int OwnerId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
}
