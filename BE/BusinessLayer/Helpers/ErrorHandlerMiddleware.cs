using System.Net;
using System.Net.Http;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace BusinessLayer.Helpers;

public class ErrorHandlerMiddleware
{
    private readonly RequestDelegate _next;

    public ErrorHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception error)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            switch (error)
            {
                case AppException e:
                    // custom application error
                    response.StatusCode = (int) HttpStatusCode.BadRequest;
                    break;
                case KeyNotFoundException e:
                    // not found error
                    response.StatusCode = (int) HttpStatusCode.NotFound;
                    break;
                case InvalidOperationException e:
                    // user access error
                    response.StatusCode = (int) HttpStatusCode.Forbidden;
                    break;
                default:
                    // unhandled error
                    response.StatusCode = (int) HttpStatusCode.InternalServerError;
                    var message = JsonSerializer.Serialize(new { message = "Internal server error!" });
                    await response.WriteAsync(message);
                    return;
            }

            var result = JsonSerializer.Serialize(new { message = error?.Message });
            await response.WriteAsync(result);
        }
    }
}