using System;
using Microsoft.Net.Http.Headers;

namespace LearnWebApi.Filters;

public class ResponseCacheFilter : IEndpointFilter
{
    private readonly int _durationInSeconds;
    private readonly string? _varyBy;
    public ResponseCacheFilter(int durationInSeconds, string? varyBy)
    {
        _durationInSeconds = durationInSeconds;
        _varyBy = varyBy;
    }
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var httpContext = context.HttpContext;
        httpContext.Response.GetTypedHeaders().CacheControl = new CacheControlHeaderValue
        {
            Public = true,
            MaxAge = TimeSpan.FromSeconds(_durationInSeconds)
        };
        if (!string.IsNullOrEmpty(_varyBy))
            httpContext.Response.Headers["Vary"] = _varyBy;

        return await next(context);
    }
}
