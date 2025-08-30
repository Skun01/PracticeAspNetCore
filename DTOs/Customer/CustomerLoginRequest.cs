using System;

namespace LearnWebApi.DTOs.Customer;

public record CustomerLoginRequest(
    string Email,
    string Password
);
