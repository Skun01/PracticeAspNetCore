using System;
using System.Threading.Tasks;
using LearnWebApi.Middlewares;
using Serilog;

namespace LearnWebApi.Extensions;

public static class RequestCountingExtentions
{

    public static IApplicationBuilder UseRequestCounting(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RequestCountingMiddlware>();
    }
}
