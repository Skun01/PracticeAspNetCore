using System;
using System.Diagnostics.Tracing;

namespace LearnWebApi.Shared;

public record Error(string Code, string Message)
{
    public static Error None = new("", "");
}

