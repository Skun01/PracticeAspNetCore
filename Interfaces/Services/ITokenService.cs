using System;
using LearnWebApi.DTOs.Customer;
using LearnWebApi.Entities;

namespace LearnWebApi.Interfaces.Services;

public interface ITokenService
{
    string CreateToken(Customer customer);
    RefreshToken CreateRefreshToken(int customerId);
    
}
