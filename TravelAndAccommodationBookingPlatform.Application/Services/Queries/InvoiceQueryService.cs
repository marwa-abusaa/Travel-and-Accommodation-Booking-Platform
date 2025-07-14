using AutoMapper;
using Microsoft.Extensions.Logging;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Invoices;
using TravelAndAccommodationBookingPlatform.Application.Interfaces.Queries;
using TravelAndAccommodationBookingPlatform.Core.Exceptions;
using TravelAndAccommodationBookingPlatform.Core.Interfaces.Repositories;
using TravelAndAccommodationBookingPlatform.Core.Interfaces.Services;

namespace TravelAndAccommodationBookingPlatform.Application.Services.Queries;

public class InvoiceQueryService : IInvoiceQueryService
{
    private readonly IInvoiceRepository _invoiceRopsitory;
    private readonly IBookingRepository _bookingRopsitory;
    private readonly IUserRepository _userRepository;
    private readonly IInvoiceHtmlBuilder _invoiceHtmlBuilder;
    private readonly IPdfGeneratorService _pdfGenerator;
    private readonly ILogger<InvoiceQueryService> _logger;
    private readonly IMapper _mapper;

    public InvoiceQueryService(
        IInvoiceRepository invoiceRopsitory,
        IBookingRepository bookingRopsitory, 
        IUserRepository userRepository, 
        IInvoiceHtmlBuilder invoiceHtmlBuilder, 
        IPdfGeneratorService pdfGenerator, 
        ILogger<InvoiceQueryService> logger, 
        IMapper mapper)
    {
        _invoiceRopsitory = invoiceRopsitory;
        _bookingRopsitory = bookingRopsitory;
        _userRepository = userRepository;
        _invoiceHtmlBuilder = invoiceHtmlBuilder;
        _pdfGenerator = pdfGenerator;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<InvoiceResponseDto?> GetInvoiceByBookingIdAsync(int bookingId)
    {
        _logger.LogInformation("Fetching invoice for booking ID {BookingId}.", bookingId);

        var booking = await _bookingRopsitory.GetBookingByIdAsync(bookingId);
        if (booking is null)
        {
            _logger.LogWarning("Booking with ID {BookingId} not found.", bookingId);
            throw new NotFoundException($"Booking with ID '{bookingId}' not found.");
        }

        var invoice = await _invoiceRopsitory.GetInvoiceByBookingIdAsync(bookingId);

        _logger.LogInformation("Invoice for booking ID {BookingId} retrieved successfully.", bookingId);

        return _mapper.Map<InvoiceResponseDto>(invoice);
    }

    public async Task<InvoiceResponseDto?> GetInvoiceByIdAsync(int invoiceId)
    {
        var invoice = await _invoiceRopsitory.GetInvoiceByIdAsync(invoiceId);
        if (invoice is null)
        {
            _logger.LogWarning("Invoice with ID {InvoiceId} not found.", invoiceId);
            throw new NotFoundException($"Invoice with ID '{invoiceId}' not found.");
        }

        _logger.LogInformation("Invoice with ID {InvoiceId} retrieved successfully.", invoiceId);

        return _mapper.Map<InvoiceResponseDto>(invoice);
    }

    public async Task<IEnumerable<InvoiceResponseDto>> GetUserInvoicesByUserIdAsync(int userId)
    {
        _logger.LogInformation("Fetching invoices for user with ID {UserId}.", userId);

        var user = await _userRepository.GetUserByIdAsync(userId);
        if (user is null)
        {
            _logger.LogWarning("User with ID {UserId} not found.", userId);
            throw new NotFoundException($"User with ID '{userId}' not found.");
        }

        var invoices = await _invoiceRopsitory.GetUserInvoicesByUserIdAsync(userId);

        _logger.LogInformation("Invoices for user with ID {UserId} retrieved successfully.", userId);

        return _mapper.Map<IEnumerable<InvoiceResponseDto>>(invoices);
    }

    public async Task<byte[]> PrintInvoice(int invoiceId)
    {
        _logger.LogInformation("Generating PDF for invoice ID {InvoiceId}.", invoiceId);

        var invoice = await _invoiceRopsitory.GetInvoiceByIdAsync(invoiceId);
        if (invoice is null)
        {
            _logger.LogWarning("Invoice with ID {InvoiceId} not found.", invoiceId);
            throw new NotFoundException($"Invoice with ID '{invoiceId}' not found.");
        }

        var htmlContent = _invoiceHtmlBuilder.BuildInvoiceHtml(invoice);
        var pdfBytes = _pdfGenerator.GeneratePdfFromHtml(htmlContent);

        _logger.LogInformation("PDF for invoice ID {InvoiceId} generated successfully.", invoiceId);

        return pdfBytes;
    }
}
