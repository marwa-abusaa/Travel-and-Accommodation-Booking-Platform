using TravelAndAccommodationBookingPlatform.Core.Entities;

namespace TravelAndAccommodationBookingPlatform.Core.Models;

public class EmailRequest
{
    public string? userEmail { get; set; }
    public string? SubjectLine { get; set; }
    public string? MessageBody { get; set; }
    public Invoice? invoice { get; set; }
    IEnumerable<FileAttachment>? FileAttachment { get; set; }
}
