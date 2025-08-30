using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace LearnWebApi.Athorization;

public class EmailDomainHandler : AuthorizationHandler<EmailDomainRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, EmailDomainRequirement requirement)
    {
        var EmailClaim = context.User.FindFirst(claim => claim.Type == ClaimTypes.Email);
        if (EmailClaim is null)
            return Task.CompletedTask;
        if (EmailClaim.Value.Contains(requirement.EmailDomain))
        {
            context.Succeed(requirement);
        }
        else
        {
            context.Fail();
        }
        return Task.CompletedTask;
    }
}
