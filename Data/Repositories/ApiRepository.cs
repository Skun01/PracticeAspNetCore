using System;
using LearnWebApi.Entities;
using LearnWebApi.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LearnWebApi.Data.Repositories;

public class ApiRepository(ProjectContext context) : IApiKeyRepository
{
     private readonly ProjectContext _context = context;

    public async Task<ApiKey> AddAsync(ApiKey entity)
    {
        _context.ApiKeys.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public Task DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<ApiKey?> GetByApiKey(string apiKey)
    {
        return await _context.ApiKeys.FirstOrDefaultAsync(a => a.Key == apiKey);
    }

    public Task<ApiKey> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(ApiKey entity)
    {
        throw new NotImplementedException();
    }
}
