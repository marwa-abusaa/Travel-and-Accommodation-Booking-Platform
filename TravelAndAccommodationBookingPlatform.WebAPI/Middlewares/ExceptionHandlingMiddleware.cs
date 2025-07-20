using TravelAndAccommodationBookingPlatform.WebAPI.Shared;

namespace TravelAndAccommodationBookingPlatform.WebAPI.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly GlobalExceptionHandler _exceptionHandler;

    public ExceptionHandlingMiddleware(RequestDelegate next, GlobalExceptionHandler exceptionHandler)
    {
        _next = next;
        _exceptionHandler = exceptionHandler;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await _exceptionHandler.TryHandleAsync(context, ex, CancellationToken.None);
        }
    }
}

