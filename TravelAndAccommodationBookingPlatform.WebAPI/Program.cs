using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Reflection;
using System.Text;
using TravelAndAccommodationBookingPlatform.Application.Validators.Cities;
using TravelAndAccommodationBookingPlatform.Core.Interfaces.Auth;
using TravelAndAccommodationBookingPlatform.Core.Models;
using TravelAndAccommodationBookingPlatform.Infrastructure.Data;
using TravelAndAccommodationBookingPlatform.Infrastructure.Services.Auth;
using TravelAndAccommodationBookingPlatform.WebAPI.Extensions;
using TravelAndAccommodationBookingPlatform.WebAPI.Middlewares;
using TravelAndAccommodationBookingPlatform.WebAPI.Shared;
using Microsoft.AspNetCore.Identity;
using TravelAndAccommodationBookingPlatform.Core.Entities;
using TravelAndAccommodationBookingPlatform.WebAPI.Validators.Authentication;


var builder = WebApplication.CreateBuilder(args);

// Configure Serilog for logging to console and file.
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
    .Enrich.FromLogContext()
    .MinimumLevel.Information()
    .CreateLogger();

builder.Host.UseSerilog();


// Register database context with SQL Server provider.
var connectionString = Environment.GetEnvironmentVariable("TravelABP_DB_CONNECTION");

if (string.IsNullOrEmpty(connectionString))
{
    throw new Exception("Connection string not found in environment variables.");
}

builder.Services.AddDbContext<AppDbContext>(options =>options.UseSqlServer(connectionString));


// Bind configuration settings for JWT and SMTP.
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));
// Register custom JWT token generator service.
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();


// Register application and infrastructure layer services.
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices();


// Configure JWT authentication.
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
var key = Encoding.UTF8.GetBytes(jwtSettings.Key);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(key),

    };
});

builder.Services.AddAuthorization();


// Register global exception handler
builder.Services.AddSingleton<GlobalExceptionHandler>();


// Configure JSON serialization for enums and custom converters.
builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
    });


// Register AutoMapper
builder.Services.AddAutoMapper(
    typeof(TravelAndAccommodationBookingPlatform.Application.Mapping.CityProfile).Assembly,
    typeof(TravelAndAccommodationBookingPlatform.WebAPI.Mapping.CityProfile).Assembly
);

// Enable FluentValidation for model validation.
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CreateCityDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<LogInRequestDtoValidator>();


// Register password hashing service.
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();


// Configure Swagger for API documentation and testing.
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "BookingPlatform API",
        Version = "v1"
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your valid token.\n\nExample: Bearer eyJhbGciOiJIUzI1NiIsInR..."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});


builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();