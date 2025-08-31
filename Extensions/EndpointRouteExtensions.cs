using System;
using LearnWebApi.Filters;

namespace LearnWebApi.Extensions;

public static class EndpointRouteExtensions
{
    public static RouteHandlerBuilder WithResponseCache(this RouteHandlerBuilder builder, int durationInSeconds, string? varyBy = null)
    {
        return builder.AddEndpointFilter(new ResponseCacheFilter(durationInSeconds, varyBy));
    }
}
