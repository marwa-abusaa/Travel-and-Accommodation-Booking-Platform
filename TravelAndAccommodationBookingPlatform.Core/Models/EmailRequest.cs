namespace TravelAndAccommodationBookingPlatform.Core.Models;

public class EmailRequest
{
    public string? ToUserEmail { get; set; }
    public string? Subject { get; set; }
    public string? MessageBody { get; set; }
    public IEnumerable<FileAttachment>? FileAttachments { get; set; }
}
