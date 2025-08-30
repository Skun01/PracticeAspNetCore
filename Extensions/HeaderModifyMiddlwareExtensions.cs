using System;
using LearnWebApi.Middlewares;

namespace LearnWebApi.Extensions;

public static class HeaderModifyMiddlwareExtensions
{
    public static IApplicationBuilder UseHeaderModify(this IApplicationBuilder builder) {
        return builder.UseMiddleware<HeaderModifyMiddleware>();
    }
}
