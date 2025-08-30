using System;
using System.Text.Json;
using FluentValidation;
using LearnWebApi.DTOs;
using LearnWebApi.DTOs.Product;
using LearnWebApi.Entities;
using LearnWebApi.Filters;
using LearnWebApi.Interfaces;
using LearnWebApi.Validators;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
namespace LearnWebApi.Endpoints;

public static class ProductEndpoints
{
    public static RouteGroupBuilder MapProductEndpoints(this WebApplication app, string routePrefix)
    {
        var group = app.MapGroup(routePrefix)
            .WithTags("Products")
            .AddEndpointFilter<LogEndpointExecutionFilter>();

        group.MapPost("/", async ([FromBody] ProductRequest request
            , IProductService productService, IValidator<ProductRequest> validator
            , HttpContext httpContext, IAntiforgery antiforgery) =>
        {
            try
            {
                await antiforgery.ValidateRequestAsync(httpContext);
                
                var validatorResult = await validator.ValidateAsync(request);
                if (validatorResult.IsValid)
                {
                    var result = await productService.CreateProductAsync(request);
                    return Results.Created($"products/{result.Value.Id}", result.Value);
                }

                var errors = validatorResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray()
                    );

                return Results.ValidationProblem(errors);
            }
            catch (AntiforgeryValidationException)
            {
                return Results.BadRequest("Invalid anti-forgery token.");
            }
            
        });

        group.MapGet("/{productId:int}", async (int productId, IProductService productService) =>
        {
            var result = await productService.GetProductByIdAsync(productId);

            return result.IsSuccess ?
                Results.Ok(result.Value)
                : Results.NotFound(result.Error);
        });

        group.MapGet("/search", async ([AsParameters] ProductQueryParameters query, IProductService productService) =>
        {
            var result = await productService.SearchProducts(query);
            return Results.Ok(result);
        });
        return group;
    }
}
