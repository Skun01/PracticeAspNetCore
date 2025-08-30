using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Serilog;

namespace LearnWebApi.Athorization;

public class MinimumAgeHandler : AuthorizationHandler<MinimumAgeRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MinimumAgeRequirement requirement)
    {
        var DateOfBirthClaim = context.User.FindFirst(c => c.Type == ClaimTypes.DateOfBirth);
        if (DateOfBirthClaim is null)
            return Task.CompletedTask;


        if (!DateTime.TryParse(DateOfBirthClaim.Value, out var DateOfBirth))
            return Task.CompletedTask;

        int calculatedAge = DateTime.Now.Year - DateOfBirth.Year;
        if (calculatedAge >= requirement.MinimumAge)
        {
            Log.Information("Authorization SUCCESSDED");
            context.Succeed(requirement);
        }
        else
        {
            Log.Information("Authorization FAILED");
            context.Fail();
        }
        return Task.CompletedTask;
    }
}
