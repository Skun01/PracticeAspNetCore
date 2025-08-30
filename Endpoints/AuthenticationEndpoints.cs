using System;
using LearnWebApi.DTOs.Customer;
using LearnWebApi.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace LearnWebApi.Endpoints;

public static class AuthenticationEndpoints
{
    public static RouteGroupBuilder MapAuthenticationEndpoints(this WebApplication app, string routePrefix)
    {
        var group = app.MapGroup(routePrefix)
            .WithTags("Authentication");
        group.MapPost("/register", async ([FromBody] CustomerRegisterRequest request ,IAuthenticationService authService) =>
        {
            var result = await authService.RegisterAsync(request);
            return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.Error);
        });

        group.MapPost("/login", async ([FromBody] CustomerLoginRequest request, IAuthenticationService authService) =>
        {
            var result = await authService.LoginAsync(request);
            return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.Error);
        });

        group.MapPost("/token/refresh", async ([FromBody] TokenRequestModel request, IAuthenticationService authService) =>
        {
            var result = await authService.RefreshToken(request);
            return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.Error);
        });
        return group;
    }
}
