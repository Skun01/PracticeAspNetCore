using System;

namespace LearnWebApi.DTOs.Customer;

public record CustomerRegisterRequest(
    string FirstName,
    string LastName,
    string Email,
    DateTime DateOfBirth,
    string Role,
    string Password
);
