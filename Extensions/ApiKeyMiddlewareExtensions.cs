using System;
using LearnWebApi.Middlewares;

namespace LearnWebApi.Extensions;

public static class ApiKeyMiddlewareExtensions
{
     public static IApplicationBuilder UseApiKeyAuthentication(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ApiKeyMiddleware>();
    }
    public static IServiceCollection AddApiKeyAuthentication(this IServiceCollection services)
    {
        // Add any required services here if needed.
        return services;
    }
}
