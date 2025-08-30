using System;
using LearnWebApi.Entities;

namespace LearnWebApi.Interfaces.Repositories;

public interface IRefreshTokenRepository : IGenericRepository<RefreshToken>
{
    Task<RefreshToken?> GetRefreshTokenByCustomerIdAsync(int customerId, string token);
}
