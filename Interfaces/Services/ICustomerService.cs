using System;
using FluentValidation;
using LearnWebApi.DTOs.Customer;
using LearnWebApi.Entities;
using LearnWebApi.Shared;

namespace LearnWebApi.Interfaces;

public interface ICustomerService
{
    Task<Customer> CreateCustomerAsync(CustomerRequest request);
}
