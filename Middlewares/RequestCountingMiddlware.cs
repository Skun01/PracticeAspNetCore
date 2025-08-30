using System;
using Serilog;

namespace LearnWebApi.Middlewares;

public class RequestCountingMiddlware : IMiddleware
{
    private int _count = 0;
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        _count++;
        Log.Information("Request Number {number}", _count);
        await next(context);
    }
}
