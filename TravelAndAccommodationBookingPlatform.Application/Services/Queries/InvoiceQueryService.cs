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
    private readonly IUserRepository _userRepository;
    private readonly IInvoiceHtmlBuilder _invoiceHtmlBuilder;
    private readonly IPdfGeneratorService _pdfGenerator;
    private readonly ILogger<InvoiceQueryService> _logger;
    private readonly IMapper _mapper;

    public InvoiceQueryService(
        IInvoiceRepository invoiceRopsitory,
        IUserRepository userRepository, 
        IInvoiceHtmlBuilder invoiceHtmlBuilder, 
        IPdfGeneratorService pdfGenerator, 
        ILogger<InvoiceQueryService> logger, 
        IMapper mapper)
    {
        _invoiceRopsitory = invoiceRopsitory;
        _userRepository = userRepository;
        _invoiceHtmlBuilder = invoiceHtmlBuilder;
        _pdfGenerator = pdfGenerator;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<InvoiceResponseDto?> GetInvoiceByIdAsync(int invoiceId, int currentUserId)
    {
        var invoice = await _invoiceRopsitory.GetInvoiceByIdAsync(invoiceId);
        if (invoice is null)
        {
            _logger.LogWarning("Invoice with ID {InvoiceId} not found.", invoiceId);
            throw new NotFoundException($"Invoice with ID '{invoiceId}' not found.");
        }

        if (invoice.Booking.UserId != currentUserId)
        {
            _logger.LogWarning("User {UserId} tried to access invoice {InvoiceId} which does not belong to them.", currentUserId, invoiceId);
            throw new ForbiddenAccessException("You are not allowed to access this invoice.");
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

    public async Task<byte[]> PrintInvoice(int invoiceId, int currentUserId)
    {
        _logger.LogInformation("Generating PDF for invoice ID {InvoiceId}.", invoiceId);

        var invoice = await _invoiceRopsitory.GetInvoiceByIdAsync(invoiceId);
        if (invoice is null)
        {
            _logger.LogWarning("Invoice with ID {InvoiceId} not found.", invoiceId);
            throw new NotFoundException($"Invoice with ID '{invoiceId}' not found.");
        }

        if (invoice.Booking.UserId != currentUserId)
        {
            _logger.LogWarning("User {UserId} tried to access invoice {InvoiceId} which does not belong to them.", currentUserId, invoiceId);
            throw new ForbiddenAccessException("You are not allowed to access this invoice.");
        }

        var htmlContent = _invoiceHtmlBuilder.BuildInvoiceHtml(invoice);
        var pdfBytes = _pdfGenerator.GeneratePdfFromHtml(htmlContent);

        _logger.LogInformation("PDF for invoice ID {InvoiceId} generated successfully.", invoiceId);

        return pdfBytes;
    }
}
