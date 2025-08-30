using System;
using Microsoft.AspNetCore.Authorization;

namespace LearnWebApi.Athorization;

public class EmailDomainRequirement : IAuthorizationRequirement
{
    public string EmailDomain { set; get; }
    public EmailDomainRequirement(string emailDomain)
    {
        EmailDomain = emailDomain;
    }
}
