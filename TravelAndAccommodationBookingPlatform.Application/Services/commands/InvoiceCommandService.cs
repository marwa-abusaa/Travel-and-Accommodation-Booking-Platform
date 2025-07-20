using AutoMapper;
using Microsoft.Extensions.Logging;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Invoices;
using TravelAndAccommodationBookingPlatform.Application.Interfaces.Commands;
using TravelAndAccommodationBookingPlatform.Core.Entities;
using TravelAndAccommodationBookingPlatform.Core.Exceptions;
using TravelAndAccommodationBookingPlatform.Core.Interfaces.Repositories;
using TravelAndAccommodationBookingPlatform.Core.Interfaces.UnitOfWork;

namespace TravelAndAccommodationBookingPlatform.Application.Services.Commands;

public class InvoiceCommandService : IInvoiceCommandService
{
    private readonly IInvoiceRepository _invoiceRopsitory;
    private readonly IBookingRepository _bookingRepository;    
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<InvoiceCommandService> _logger;
    private readonly IMapper _mapper;

    public InvoiceCommandService(
        IInvoiceRepository invoiceRopsitory, 
        IBookingRepository bookingRepository,
        IUnitOfWork unitOfWork, 
        ILogger<InvoiceCommandService> logger,
        IMapper mapper)
    {
        _invoiceRopsitory = invoiceRopsitory;
        _bookingRepository = bookingRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<InvoiceResponseDto> AddInvoiceAsync(CreateInvoiceDto createInvoiceDto)
    {
        _logger.LogInformation("Attempting to add invoice for booking ID {BookingId}.", createInvoiceDto.BookingId);

        var booking = await _bookingRepository.GetBookingByIdAsync(createInvoiceDto.BookingId);
        if (booking is null)
        {
            _logger.LogWarning("Booking with ID {BookingId} not found.", createInvoiceDto.BookingId);
            throw new NotFoundException($"Booking with ID '{createInvoiceDto.BookingId}' not found.");
        }

        var invoice = _mapper.Map<Invoice>(createInvoiceDto);
        invoice.TotalAmount = booking.TotalPriceAfterDiscount;
        var addedInvoice = await _invoiceRopsitory.AddInvoiceAsync(invoice);

        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Invoice with ID {InvoiceId} added successfully.", addedInvoice.InvoiceId);

        return _mapper.Map<InvoiceResponseDto>(addedInvoice);
    }

}
