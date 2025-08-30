using System;
using LearnWebApi.Entities;

namespace LearnWebApi.Interfaces.Repositories;

public interface ICustomerRepository : IGenericRepository<Customer>
{
    Task<bool> IsCustomerEmailExistAsync(string email);
    Task<string> GetHashPasswordByEmailAsync(string email);
    Task<Customer> GetCustomerByEmailAsync(string email);
}
