using System;

namespace LearnWebApi.Interfaces.Services;

public interface ICacheService
{
    T? Get<T>(string key);
    void Set<T>(string key, T value, TimeSpan duration, bool sliding = false);
    void Remove(string key);
}
