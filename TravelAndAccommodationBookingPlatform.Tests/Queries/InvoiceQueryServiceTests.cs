using FluentAssertions;
using Moq;
using TravelAndAccommodationBookingPlatform.Application.Services.Queries;
using TravelAndAccommodationBookingPlatform.Core.Entities;
using TravelAndAccommodationBookingPlatform.Core.Exceptions;
using TravelAndAccommodationBookingPlatform.Core.Interfaces.Repositories;
using Microsoft.Extensions.Logging;
using AutoMapper;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Invoices;
using TravelAndAccommodationBookingPlatform.Core.Interfaces.Services;


namespace TravelAndAccommodationBookingPlatform.Tests.Queries;

public class InvoiceQueryServiceTests
{
    private readonly Mock<IInvoiceRepository> _invoiceRepoMock = new();
    private readonly Mock<IUserRepository> _userRepoMock = new();
    private readonly Mock<IInvoiceHtmlBuilder> _htmlBuilderMock = new();
    private readonly Mock<IPdfGeneratorService> _pdfGeneratorMock = new();
    private readonly Mock<ILogger<InvoiceQueryService>> _loggerMock = new();
    private readonly Mock<IMapper> _mapperMock = new();

    private InvoiceQueryService CreateService() =>
        new InvoiceQueryService(
            _invoiceRepoMock.Object,
            _userRepoMock.Object,
            _htmlBuilderMock.Object,
            _pdfGeneratorMock.Object,
            _loggerMock.Object,
            _mapperMock.Object);

    [Fact]
    public async Task GetInvoiceByIdAsync_ShouldReturnInvoice_WhenInvoiceExistsAndUserAuthorized()
    {
        // Arrange
        var service = CreateService();
        int invoiceId = 1;
        int userId = 10;

        var invoice = new Invoice
        {
            InvoiceId = invoiceId,
            Booking = new Booking { UserId = userId }
        };

        var expectedDto = new InvoiceResponseDto { InvoiceId = invoiceId };

        _invoiceRepoMock.Setup(x => x.GetInvoiceByIdAsync(invoiceId)).ReturnsAsync(invoice);
        _mapperMock.Setup(m => m.Map<InvoiceResponseDto>(invoice)).Returns(expectedDto);

        // Act
        var result = await service.GetInvoiceByIdAsync(invoiceId, userId);

        // Assert
        result.Should().BeEquivalentTo(expectedDto);
        _invoiceRepoMock.Verify(x => x.GetInvoiceByIdAsync(invoiceId), Times.Once);
    }

    [Fact]
    public async Task GetInvoiceByIdAsync_ShouldThrowNotFoundException_WhenInvoiceNotFound()
    {
        var service = CreateService();
        int invoiceId = 1;
        int userId = 10;

        _invoiceRepoMock.Setup(x => x.GetInvoiceByIdAsync(invoiceId)).ReturnsAsync((Invoice?)null);

        Func<Task> act = async () => await service.GetInvoiceByIdAsync(invoiceId, userId);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Invoice with ID '{invoiceId}' not found.");
    }


    [Fact]
    public async Task GetUserInvoicesByUserIdAsync_ShouldReturnInvoices_WhenUserExists()
    {
        var service = CreateService();
        int userId = 10;

        var user = new User { UserId = userId };
        var invoices = new List<Invoice>
        {
            new Invoice { InvoiceId = 1 },
            new Invoice { InvoiceId = 2 }
        };

        var mappedDtos = new List<InvoiceResponseDto>
        {
            new InvoiceResponseDto { InvoiceId = 1 },
            new InvoiceResponseDto { InvoiceId = 2 }
        };

        _userRepoMock.Setup(x => x.GetUserByIdAsync(userId)).ReturnsAsync(user);
        _invoiceRepoMock.Setup(x => x.GetUserInvoicesByUserIdAsync(userId)).ReturnsAsync(invoices);
        _mapperMock.Setup(m => m.Map<IEnumerable<InvoiceResponseDto>>(invoices)).Returns(mappedDtos);

        var result = await service.GetUserInvoicesByUserIdAsync(userId);

        result.Should().BeEquivalentTo(mappedDtos);
        _userRepoMock.Verify(x => x.GetUserByIdAsync(userId), Times.Once);
        _invoiceRepoMock.Verify(x => x.GetUserInvoicesByUserIdAsync(userId), Times.Once);
    }

    [Fact]
    public async Task GetUserInvoicesByUserIdAsync_ShouldThrowNotFoundException_WhenUserNotFound()
    {
        var service = CreateService();
        int userId = 10;

        _userRepoMock.Setup(x => x.GetUserByIdAsync(userId)).ReturnsAsync((User?)null);

        Func<Task> act = async () => await service.GetUserInvoicesByUserIdAsync(userId);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"User with ID '{userId}' not found.");
    }


    [Fact]
    public async Task PrintInvoice_ShouldReturnPdfBytes_WhenInvoiceExistsAndUserAuthorized()
    {
        var service = CreateService();
        int invoiceId = 1;
        int userId = 10;

        var invoice = new Invoice
        {
            InvoiceId = invoiceId,
            Booking = new Booking { UserId = userId }
        };

        byte[] pdfBytes = new byte[] { 0x1, 0x2, 0x3 };
        string htmlContent = "<html>invoice</html>";

        _invoiceRepoMock.Setup(x => x.GetInvoiceByIdAsync(invoiceId)).ReturnsAsync(invoice);
        _htmlBuilderMock.Setup(b => b.BuildInvoiceHtml(invoice)).Returns(htmlContent);
        _pdfGeneratorMock.Setup(p => p.GeneratePdfFromHtml(htmlContent)).Returns(pdfBytes);

        var result = await service.PrintInvoice(invoiceId, userId);

        result.Should().BeEquivalentTo(pdfBytes);
        _invoiceRepoMock.Verify(x => x.GetInvoiceByIdAsync(invoiceId), Times.Once);
        _htmlBuilderMock.Verify(b => b.BuildInvoiceHtml(invoice), Times.Once);
        _pdfGeneratorMock.Verify(p => p.GeneratePdfFromHtml(htmlContent), Times.Once);
    }


    [Fact]
    public async Task PrintInvoice_ShouldThrowNotFoundException_WhenInvoiceNotFound()
    {
        var service = CreateService();
        int invoiceId = 1;
        int userId = 10;

        _invoiceRepoMock.Setup(x => x.GetInvoiceByIdAsync(invoiceId)).ReturnsAsync((Invoice?)null);

        Func<Task> act = async () => await service.PrintInvoice(invoiceId, userId);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Invoice with ID '{invoiceId}' not found.");
    }

    [Fact]
    public async Task PrintInvoice_ShouldThrowForbiddenAccessException_WhenUserNotAuthorized()
    {
        var service = CreateService();
        int invoiceId = 1;
        int userId = 10;
        int otherUserId = 99;

        var invoice = new Invoice
        {
            InvoiceId = invoiceId,
            Booking = new Booking { UserId = otherUserId }
        };

        _invoiceRepoMock.Setup(x => x.GetInvoiceByIdAsync(invoiceId)).ReturnsAsync(invoice);

        Func<Task> act = async () => await service.PrintInvoice(invoiceId, userId);

        await act.Should().ThrowAsync<ForbiddenAccessException>()
            .WithMessage("You are not allowed to access this invoice.");
    }
}
