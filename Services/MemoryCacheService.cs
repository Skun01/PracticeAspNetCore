using System;
using LearnWebApi.Interfaces.Services;
using Microsoft.Extensions.Caching.Memory;
using Serilog;

namespace LearnWebApi.Services;

public class MemoryCacheService : ICacheService
{
    private readonly IMemoryCache _memoryCache;
    public MemoryCacheService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public T? Get<T>(string key)
    {
        bool found = _memoryCache.TryGetValue(key, out T? value);
        Log.Information("Cache {status} for key '{key}'", found ? "HIT" : "MIT", key);
        return value;
    }

    public void Remove(string key)
    {
        _memoryCache.Remove(key);
        Log.Information($"Removed cache for key '{key}'");
    }

    public void Set<T>(string key, T value, TimeSpan duration, bool sliding = false)
    {
        var options = new MemoryCacheEntryOptions();
        if (sliding)
            options.SetAbsoluteExpiration(duration);
        else
            options.SetSlidingExpiration(duration);
        _memoryCache.Set(key, value, options);
        Log.Information($"Cached key '{key}' for {duration} seconds");
    }
}
