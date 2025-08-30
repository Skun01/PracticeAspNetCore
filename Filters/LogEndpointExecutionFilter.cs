using System;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LearnWebApi.Filters;

public class LogEndpointExecutionFilter : IEndpointFilter
{
    private readonly ILogger<LogEndpointExecutionFilter> _logger;
    public LogEndpointExecutionFilter(ILogger<LogEndpointExecutionFilter> logger)
    {
        _logger = logger;
    }

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        _logger.LogInformation("Endpoint {displayName} Start...", context.HttpContext.Request.Path);
        var result = await next(context);
        _logger.LogInformation("...Endpoint {displayName} End", context.HttpContext.Request.Path);
        return result;
    }
}
