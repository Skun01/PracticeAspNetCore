using System;
using FluentValidation;
using LearnWebApi.DTOs.Customer;
using LearnWebApi.Entities;
using LearnWebApi.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LearnWebApi.Endpoints;

public static class CustomerEndpoints
{
    public static RouteGroupBuilder MapCustomerEndpoints(this WebApplication app, string routePrefix)
    {
        var group = app.MapGroup(routePrefix)
            .WithName("Customers");

        // CREATE CUSTOMER
        group.MapPost("/", async ([FromBody] CustomerRequest request, ICustomerService customerService, IValidator<CustomerRequest> validator) =>
        {
            var validationResult = await validator.ValidateAsync(request);
            if (validationResult.IsValid)
            {
                Customer newCustomer = await customerService.CreateCustomerAsync(request);
                return Results.Ok(newCustomer);
            }

            var errors = validationResult.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray()
                );
            return Results.ValidationProblem(errors);
        });
        return group;
    }
}
