using System;
using LearnWebApi.Interfaces.Repositories;
using Serilog;

namespace LearnWebApi.Middlewares;

public class ApiKeyMiddleware (IApiKeyRepository apiKeyRepository) : IMiddleware
{
    
    public const string ApiKeyHeaderName = "X-API-Key";
    private readonly IApiKeyRepository _apiKeyRepository = apiKeyRepository;
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (!context.Request.Headers.TryGetValue(ApiKeyHeaderName, out var extractedApiKey))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsJsonAsync("API key missing");
            Log.Warning("API key missing in request");
            return;
        }
        var apiKey = extractedApiKey.FirstOrDefault();
        if (string.IsNullOrEmpty(apiKey))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("API Key cannot be empty");
            Log.Warning("API Key is empty.");
            return;
        }
        // Hash the API Key and compare it with the stored hash
        var apiKeyObject = await _apiKeyRepository.GetByApiKey(apiKey);
        if (apiKeyObject is null || apiKeyObject.IsActive == false)
        {
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync("Server configuration error: StoredApiKeyHash is not set.");
            Log.Error("Server configuration error: StoredApiKeyHash is not set.");
            return;
        }
        await next(context); // Proceed to the next middleware
    }
}
