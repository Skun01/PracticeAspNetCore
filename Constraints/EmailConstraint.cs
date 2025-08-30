using System;
using System.Text.RegularExpressions;

namespace LearnWebApi.Constraints;

public class EmailConstraint : IRouteConstraint
{
    public bool Match(HttpContext? httpContext, IRouter? route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
    {
        string regex = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
        return Regex.IsMatch(values[routeKey]?.ToString() ?? "", regex);
    }
}
