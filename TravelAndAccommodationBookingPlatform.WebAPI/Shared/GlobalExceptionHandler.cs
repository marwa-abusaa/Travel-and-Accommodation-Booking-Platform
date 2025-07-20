using Microsoft.AspNetCore.Diagnostics;
using System.Diagnostics;
using TravelAndAccommodationBookingPlatform.Core.Exceptions;

namespace TravelAndAccommodationBookingPlatform.WebAPI.Shared;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        LogException(exception);

        var (statusCode, title, detail) = MapExceptionToProblemInformation(exception);

        await Results.Problem(
           statusCode: statusCode,
           title: title,
           detail: detail,
           extensions: new Dictionary<string, object?>
           {
               ["traceId"] = Activity.Current?.Id ?? httpContext.TraceIdentifier
           })
           .ExecuteAsync(httpContext);

        return true;
    }

    private void LogException(Exception exception)
    {
        if (IsCustomException(exception))
        {
            _logger.LogWarning(exception, exception.Message);
        }
        else
        {
            _logger.LogError(exception, "An unexpected error occurred.");
        }
    }

    private static bool IsCustomException(Exception exception)
    {
        return exception is BadRequestException ||
               exception is ConflictException ||
               exception is EmailAlreadyExistsException ||
               exception is InvalidCredentialsException ||
               exception is NotFoundException;
    }

    private static (int statusCode, string title, string detail) MapExceptionToProblemInformation(Exception exception)
    {
        return exception switch
        {
            BadRequestException => (StatusCodes.Status400BadRequest, "Bad Request", exception.Message),
            ConflictException => (StatusCodes.Status409Conflict, "Conflict", exception.Message),
            EmailAlreadyExistsException => (StatusCodes.Status400BadRequest, "Email Already Exists", exception.Message),
            InvalidCredentialsException => (StatusCodes.Status401Unauthorized, "Invalid Credentials", exception.Message),
            NotFoundException => (StatusCodes.Status404NotFound, "Not Found", exception.Message),
            ArgumentException => (StatusCodes.Status400BadRequest, "Invalid Argument", exception.Message),
            ForbiddenAccessException => (StatusCodes.Status403Forbidden, "Unauthorized Access", exception.Message),

            _ => (
                StatusCodes.Status500InternalServerError,
                "Internal Server Error",
                "An unexpected error occurred on the server."
            )
        };
    }
}
