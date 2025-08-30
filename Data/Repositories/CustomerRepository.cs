using System;
using LearnWebApi.Entities;
using LearnWebApi.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LearnWebApi.Data.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly ProjectContext _context;
    public CustomerRepository(ProjectContext context)
    {
        _context = context;
    }
    public async Task<Customer> AddAsync(Customer entity)
    {
        bool isEmailExist = await IsCustomerEmailExistAsync(entity.Email);
        if (isEmailExist)
            throw new ArgumentException($"Your email already have account");

        _context.Customers.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<Customer> GetCustomerByEmailAsync(string email)
    {
        Customer? customer = await _context.Customers
        .AsNoTracking()
        .FirstOrDefaultAsync(c => c.Email == email);
        if (customer is null)
            throw new KeyNotFoundException("Customer not found!");

        return customer;
    }

    public Task DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<Customer> GetByIdAsync(int id)
    {
        Customer? target = await _context.Customers.FirstOrDefaultAsync(c => c.Id == id);
        if (target is null)
            throw new KeyNotFoundException("Customer not found!");
        return target;
    }

    public async Task<string> GetHashPasswordByEmailAsync(string email)
    {
        Customer customer = await GetCustomerByEmailAsync(email);
        return customer.PasswordHash;
    }

    public async Task<bool> IsCustomerEmailExistAsync(string email)
    {
        Customer? customer = await _context.Customers.FirstOrDefaultAsync(c => c.Email == email);
        return customer is null ? false : true;
    }

    public Task UpdateAsync(Customer entity)
    {
        throw new NotImplementedException();
    }
}
