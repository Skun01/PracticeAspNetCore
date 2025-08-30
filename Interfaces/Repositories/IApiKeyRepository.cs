using System;
using LearnWebApi.Entities;

namespace LearnWebApi.Interfaces.Repositories;

public interface IApiKeyRepository : IGenericRepository<ApiKey>
{
    Task<ApiKey?> GetByApiKey(string apiKey);
}
