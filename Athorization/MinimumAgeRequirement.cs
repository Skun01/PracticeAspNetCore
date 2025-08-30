using System;
using Microsoft.AspNetCore.Authorization;

namespace LearnWebApi.Athorization;

public class MinimumAgeRequirement : IAuthorizationRequirement
{
    public int MinimumAge { set; get; }
    public MinimumAgeRequirement(int minimumAge)
    {
        MinimumAge = minimumAge;
    }

}
