using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Invoices;
using TravelAndAccommodationBookingPlatform.Application.Interfaces.Commands;
using TravelAndAccommodationBookingPlatform.Application.Interfaces.Queries;
using TravelAndAccommodationBookingPlatform.WebAPI.Dtos.Invoices;
using TravelAndAccommodationBookingPlatform.WebAPI.Extensions;
using TravelAndAccommodationBookingPlatform.WebAPI.Shared;

namespace TravelAndAccommodationBookingPlatform.WebAPI.Controllers;

[ApiController]
[Route("api/bookings/{bookingId:int}/invoices")]
[Authorize(Roles = UserRoles.User)]
public class InvoicesController : ControllerBase
{
    private readonly IInvoiceQueryService _invoiceQueryService;
    private readonly IInvoiceCommandService _invoiceCommandService;
    private readonly IMapper _mapper;

    public InvoicesController(
        IInvoiceQueryService invoiceQueryService, 
        IInvoiceCommandService invoiceCommandService, 
        IMapper mapper)
    {
        _invoiceQueryService = invoiceQueryService;
        _invoiceCommandService = invoiceCommandService;
        _mapper = mapper;
    }


    /// <summary>
    /// Get invoice by ID.
    /// </summary>
    /// <param name="invoiceId">The ID of invoice.</param>
    /// <returns>The invoice details.</returns>
    /// <response code="200">Invoice found.</response>
    /// <response code="404">Invoice not found.</response>
    /// <response code="401">Unauthorized.</response>
    [HttpGet("{invoiceId:int}")]
    [ProducesResponseType(typeof(InvoiceResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]

    public async Task<ActionResult<InvoiceResponseDto>> GetInvoice(int invoiceId)
    {
        int userId = User.GetUserId();
        var invoice = await _invoiceQueryService.GetInvoiceByIdAsync(invoiceId, userId);
        return Ok(invoice);
    }


    /// <summary>
    /// Create a new invoice.
    /// </summary>
    /// <param name="bookingId">The ID of the booking associated with the invoice.</param>
    /// <param name="request"> Invoice details.</param>
    /// <returns>The created invoice.</returns>
    /// <response code="201">Invoice created successfully.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="403">Forbidden (Not Current User).</response>
    [HttpPost]
    [ProducesResponseType(typeof(InvoiceResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<InvoiceResponseDto>> CreateInvoice(int bookingId, [FromBody]InvoiceCreationRequestDto request)
    {
        var dto = _mapper.Map<CreateInvoiceDto>(request);
        dto.BookingId = bookingId;
        var createdInvoice = await _invoiceCommandService.AddInvoiceAsync(dto);
        return CreatedAtAction(nameof(GetInvoice), new { bookingId= bookingId, invoiceId = createdInvoice.InvoiceId }, createdInvoice);
    }


    /// <summary>
    /// Generate and download a PDF invoice for the authenticated user.
    /// </summary>
    /// <param name="invoiceId">The ID of invoice to print.</param>
    /// <returns>PDF file of the invoice.</returns>
    /// <response code="200">Invoice generated successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized to access this invoice.</response>
    /// <response code="404">Invoice not found.</response>
    [HttpGet("{invoiceId:int}/print")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<FileResult> PrintInvoice(int invoiceId)
    {
        int userId = User.GetUserId();
        var pdfBytes = await _invoiceQueryService.PrintInvoice(invoiceId, userId);
        return File(pdfBytes, "application/pdf", $"invoice-{invoiceId}.pdf");
    }
}
