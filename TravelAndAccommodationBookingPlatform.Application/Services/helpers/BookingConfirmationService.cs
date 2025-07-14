using TravelAndAccommodationBookingPlatform.Core.Entities;
using TravelAndAccommodationBookingPlatform.Core.Interfaces.Services;
using TravelAndAccommodationBookingPlatform.Core.Models;

namespace TravelAndAccommodationBookingPlatform.Application.Services.Helpers;

public class BookingConfirmationService : IBookingConfirmationService
{
    private readonly BookingHtmlBuilder _htmlBuilder;
    private readonly IPdfGeneratorService _pdfGenerator;
    private readonly IEmailService _emailService;

    public BookingConfirmationService(BookingHtmlBuilder htmlBuilder,
        IPdfGeneratorService pdfGenerator, IEmailService emailService)
    {
        _htmlBuilder = htmlBuilder;
        _pdfGenerator = pdfGenerator;
        _emailService = emailService;
    }

    public async Task SendBookingConfirmationAsync(Booking booking)
    {
        var html = _htmlBuilder.BuildConfirmationHtml(booking);        
        var pdf = _pdfGenerator.GeneratePdfFromHtml(html);


        var emailRequest = new EmailRequest
        {
            ToUserEmail = booking.User.Email,
            Subject = "Booking Confirmation",
            MessageBody = "Your booking is confirmed. Thank you for booking with us. Please find the confirmation attached.",
            FileAttachments = new List<FileAttachment>
            {
                new FileAttachment { FileName = "confirmation.pdf", ContentType = "application/pdf", Content = pdf }
            }
        };

        
        await _emailService.SendEmailAsync(emailRequest);
    }
}
