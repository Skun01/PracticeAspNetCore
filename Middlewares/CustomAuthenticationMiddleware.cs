using System;

namespace LearnWebApi.Middlewares;

public class CustomAuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;
    public CustomAuthenticationMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _configuration = configuration;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Headers.ContainsKey("Authorization"))
        {
            string apiKey = context.Request.Headers["Authorization"].ToString();
            if (apiKey == _configuration["ApiKey"])
            {
                await _next(context);
            }
            else
            {
                context.Response.StatusCode = 401;
                return;
            }
        }
        else
        {
            context.Response.StatusCode = 401;
            return;
        }
        
    }
}
