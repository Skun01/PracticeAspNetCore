using System;
using LearnWebApi.Entities;
using LearnWebApi.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LearnWebApi.Data.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly ProjectContext _context;
    public RefreshTokenRepository(ProjectContext context)
    {
        _context = context;
    }

    public async Task<RefreshToken?> GetRefreshTokenByCustomerIdAsync(int customerId, string token)
    {
        return await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == token && rt.CustomerId == customerId && !rt.IsRevoked);
    }
    public async Task<RefreshToken> AddAsync(RefreshToken entity)
    {
        _context.RefreshTokens.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public Task DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<RefreshToken> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task UpdateAsync(RefreshToken entity)
    {
        _context.RefreshTokens.Update(entity);
        await _context.SaveChangesAsync();
    }
}
