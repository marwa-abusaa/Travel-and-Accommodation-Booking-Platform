using Microsoft.EntityFrameworkCore;
using TravelAndAccommodationBookingPlatform.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = Environment.GetEnvironmentVariable("TravelABP_DB_CONNECTION");

if (string.IsNullOrEmpty(connectionString))
{
    throw new Exception("Connection string not found in environment variables.");
}

builder.Services.AddDbContext<AppDbContext>(options =>options.UseSqlServer(connectionString));

//builder.Services.AddControllers();
//builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

//app.UseAuthorization();

//app.MapControllers();

app.Run();
