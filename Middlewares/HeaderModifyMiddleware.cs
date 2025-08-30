using System;

namespace LearnWebApi.Middlewares;

public class HeaderModifyMiddleware
{
    private readonly RequestDelegate _next;
    public HeaderModifyMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    public async Task InvokeAsync(HttpContext httpContext)
    {
        httpContext.Response.Headers["DateTime"] = DateTime.UtcNow.ToString("dd:MM:yyy hh:mm:ss");
        await _next(httpContext);
    }
}
