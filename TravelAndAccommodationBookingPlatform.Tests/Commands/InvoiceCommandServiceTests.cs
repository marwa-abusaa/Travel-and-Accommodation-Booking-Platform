using FluentAssertions;
using Moq;
using TravelAndAccommodationBookingPlatform.Application.Services.Commands;
using TravelAndAccommodationBookingPlatform.Core.Entities;
using TravelAndAccommodationBookingPlatform.Core.Exceptions;
using TravelAndAccommodationBookingPlatform.Core.Interfaces.Repositories;
using Microsoft.Extensions.Logging;
using AutoMapper;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Invoices;
using TravelAndAccommodationBookingPlatform.Core.Enums;
using TravelAndAccommodationBookingPlatform.Core.Interfaces.UnitOfWork;


namespace TravelAndAccommodationBookingPlatform.Tests.Commands;

public class InvoiceCommandServiceTests
{
    private readonly Mock<IInvoiceRepository> _invoiceRepoMock = new();
    private readonly Mock<IBookingRepository> _bookingRepoMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<ILogger<InvoiceCommandService>> _loggerMock = new();
    private readonly Mock<IMapper> _mapperMock = new();

    private InvoiceCommandService CreateService() =>
        new InvoiceCommandService(
            _invoiceRepoMock.Object,
            _bookingRepoMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object,
            _mapperMock.Object);

    [Fact]
    public async Task AddInvoiceAsync_ShouldReturnInvoice_WhenBookingExists()
    {
        // Arrange
        var service = CreateService();
        var createDto = new CreateInvoiceDto
        {
            BookingId = 123,
            PaymentStatus = PaymentStatus.Paid,
            Notes = "Test notes"
        };

        var booking = new Booking
        {
            BookingId = 123,
            TotalPriceAfterDiscount = 500m
        };

        var invoiceEntity = new Invoice
        {
            InvoiceId = 1,
            BookingId = 123,
            InvoiceDate = createDto.InvoiceDate,
            TotalAmount = 500m,
            PaymentStatus = createDto.PaymentStatus,
            Notes = createDto.Notes
        };

        var expectedResponse = new InvoiceResponseDto
        {
            InvoiceId = 1,
            BookingId = 123,
            InvoiceDate = createDto.InvoiceDate,
            TotalAmount = 500m,
            PaymentStatus = createDto.PaymentStatus,
            Notes = createDto.Notes
        };

        _bookingRepoMock.Setup(b => b.GetBookingByIdAsync(createDto.BookingId)).ReturnsAsync(booking);

        _mapperMock.Setup(m => m.Map<Invoice>(createDto)).Returns(invoiceEntity);

        _invoiceRepoMock.Setup(i => i.AddInvoiceAsync(invoiceEntity)).ReturnsAsync(invoiceEntity);       

        _mapperMock.Setup(m => m.Map<InvoiceResponseDto>(invoiceEntity)).Returns(expectedResponse);

        // Act
        var result = await service.AddInvoiceAsync(createDto);

        // Assert
        result.Should().BeEquivalentTo(expectedResponse);

        _bookingRepoMock.Verify(b => b.GetBookingByIdAsync(createDto.BookingId), Times.Once);
        _invoiceRepoMock.Verify(i => i.AddInvoiceAsync(invoiceEntity), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        _mapperMock.Verify(m => m.Map<Invoice>(createDto), Times.Once);
        _mapperMock.Verify(m => m.Map<InvoiceResponseDto>(invoiceEntity), Times.Once);
    }

    [Fact]
    public async Task AddInvoiceAsync_ShouldThrowNotFoundException_WhenBookingDoesNotExist()
    {
        // Arrange
        var service = CreateService();
        var createDto = new CreateInvoiceDto
        {
            BookingId = 999,
            InvoiceDate = DateTime.UtcNow,
            PaymentStatus = PaymentStatus.Paid
        };

        _bookingRepoMock.Setup(b => b.GetBookingByIdAsync(createDto.BookingId)).ReturnsAsync((Booking?)null);

        // Act
        Func<Task> act = async () => await service.AddInvoiceAsync(createDto);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
                 .WithMessage($"Booking with ID '{createDto.BookingId}' not found.");

        _bookingRepoMock.Verify(b => b.GetBookingByIdAsync(createDto.BookingId), Times.Once);
        _invoiceRepoMock.Verify(i => i.AddInvoiceAsync(It.IsAny<Invoice>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
    }
}