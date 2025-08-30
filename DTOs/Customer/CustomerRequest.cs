using System;
using System.ComponentModel.DataAnnotations;

namespace LearnWebApi.DTOs.Customer;

public record CustomerRequest(
    string FirstName,
    string LastName,
    string Email,
    DateTime DateOfBirth
);