namespace TravelAndAccommodationBookingPlatform.Core.Entities;

public class User
{
    public int UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public int RoleId { get; set; }
    public Role Role { get; set; }
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();

}
