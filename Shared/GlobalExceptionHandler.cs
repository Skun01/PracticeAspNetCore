using System;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace LearnWebApi.Shared;

public class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "System error",
            Detail = "An unexpected error occured.",
            Instance = httpContext.Request.Path
        };

        // Đính traceId để tra log
        var traceId = httpContext.TraceIdentifier;
        problemDetails.Extensions["traceId"] = traceId;

        // Ghi log lỗi có cấu trúc để tra cứu về sau
        Log.ForContext("traceId", traceId)
           .ForContext("path", httpContext.Request.Path)
           .Error(exception, "Unhandled exception");

        httpContext.Response.StatusCode = problemDetails.Status.Value;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        return true;
    }
}
