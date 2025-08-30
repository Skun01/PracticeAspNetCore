using System;
using LearnWebApi.Middlewares;

namespace LearnWebApi.Extensions;

public static class CustomAuthenticationExtensions
{
    public static IApplicationBuilder UseCustomAuthentication(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<CustomAuthenticationMiddleware>();
    }
}
