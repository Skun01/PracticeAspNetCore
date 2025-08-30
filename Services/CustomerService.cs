using System;
using FluentValidation;
using LearnWebApi.Data;
using LearnWebApi.Data.Repositories;
using LearnWebApi.DTOs.Customer;
using LearnWebApi.Entities;
using LearnWebApi.Interfaces;
using LearnWebApi.Interfaces.Repositories;
using LearnWebApi.Shared;

namespace LearnWebApi.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IValidator<CustomerRequest> _validator;
    public CustomerService(ICustomerRepository customerRepository, IValidator<CustomerRequest> validator)
    {
        _customerRepository = customerRepository;
        _validator = validator;
    }
    public async Task<Customer> CreateCustomerAsync(CustomerRequest request)
    {
        Console.WriteLine("Nothing to print: " + request.DateOfBirth.ToString());
        Customer newCustomer = new()
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            DateOfBirth = request.DateOfBirth
        };
        newCustomer = await _customerRepository.AddAsync(newCustomer);
        return newCustomer;
    }
}
