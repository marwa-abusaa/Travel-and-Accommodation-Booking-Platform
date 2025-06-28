namespace TravelAndAccommodationBookingPlatform.Core.Models;

public class FileAttachment
{
    public string? FileName { get; set; }
    public string? ContentType { get; set; }
    public byte[]? Content { get; set; }   
}
