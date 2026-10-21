using Backend.Application.Common;
using FluentValidation;

namespace Backend.API.Middlewares;

public sealed class GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException ex)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(ApiResponse<object>.Fail("Validation failed", ex.Errors.Select(x => x.ErrorMessage).ToList()));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled API error");
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(ApiResponse<object>.Fail("Internal server error"));
        }
    }
}
