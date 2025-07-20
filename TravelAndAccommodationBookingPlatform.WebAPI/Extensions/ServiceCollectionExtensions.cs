using Sieve.Services;
using TravelAndAccommodationBookingPlatform.Application.Interfaces.Commands;
using TravelAndAccommodationBookingPlatform.Application.Interfaces.Queries;
using TravelAndAccommodationBookingPlatform.Application.Services.Commands;
using TravelAndAccommodationBookingPlatform.Application.Services.Helpers;
using TravelAndAccommodationBookingPlatform.Application.Services.Queries;
using TravelAndAccommodationBookingPlatform.Core.Interfaces.Repositories;
using TravelAndAccommodationBookingPlatform.Core.Interfaces.Services;
using TravelAndAccommodationBookingPlatform.Core.Interfaces.UnitOfWork;
using TravelAndAccommodationBookingPlatform.Infrastructure.Repositories;
using TravelAndAccommodationBookingPlatform.Infrastructure.Services;
using TravelAndAccommodationBookingPlatform.Infrastructure.UnitOfWork;

namespace TravelAndAccommodationBookingPlatform.WebAPI.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IBookingCommandService, BookingCommandService>();
        services.AddScoped<IBookingQueryService, BookingQueryService>();

        services.AddScoped<ICityCommandService, CityCommandService>();
        services.AddScoped<ICityQueryService, CityQueryService>();

        services.AddScoped<IDiscountCommandService, DiscountCommandService>();
        services.AddScoped<IDiscountQueryService, DiscountQueryService>();

        services.AddScoped<IHotelCommandService, HotelCommandService>();
        services.AddScoped<IHotelQueryService, HotelQueryService>();

        services.AddScoped<IImageCommandService, ImageCommandService>();
        services.AddScoped<IImageQueryService, ImageQueryService>();

        services.AddScoped<IInvoiceCommandService, InvoiceCommandService>();
        services.AddScoped<IInvoiceQueryService, InvoiceQueryService>();

        services.AddScoped<IOwnerCommandService, OwnerCommandService>();
        services.AddScoped<IOwnerQueryService, OwnerQueryService>();

        services.AddScoped<IReviewCommandService, ReviewCommandService>();
        services.AddScoped<IReviewQueryService, ReviewQueryService>();

        services.AddScoped<IRoomCommandService, RoomCommandService>();
        services.AddScoped<IRoomQueryService, RoomQueryService>();

        services.AddScoped<IUserCommandService, UserCommandService>();
        services.AddScoped<IUserQueryService, UserQueryService>();


        services.AddScoped<ISieveProcessor, SieveProcessor>();


        services.AddScoped<IBookingConfirmationService, BookingConfirmationService>();
        services.AddScoped<IBookingCreationService, BookingCreationService>();
        services.AddScoped<IBookingHtmlBuilder, BookingHtmlBuilder>();
        services.AddScoped<IInvoiceHtmlBuilder, InvoiceHtmlBuilder>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddSingleton<IPdfGeneratorService, PdfGeneratorService>();

        return services;
    }

    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IBookingRepository, BookingRepository>();
        services.AddScoped<ICityRepository, CityRepository>();
        services.AddScoped<IDiscountRepository, DiscountRepository>();
        services.AddScoped<IHotelRepository, HotelRepository>();
        services.AddScoped<IImageRepository, ImageRepository>();
        services.AddScoped<IInvoiceRepository, InvoiceRepository>();
        services.AddScoped<IOwnerRepository, OwnerRepository>();
        services.AddScoped<IReviewRepository, ReviewRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IRoomRepository, RoomRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }
}
