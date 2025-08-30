using System;
using FluentValidation;
using LearnWebApi.DTOs.Customer;

namespace LearnWebApi.Validators;

public class CustomerRequestValidator : AbstractValidator<CustomerRequest>
{
    public CustomerRequestValidator()
    {
        RuleFor(request => request.FirstName)
            .NotEmpty().WithMessage("First Name is required.")
            .MaximumLength(50).WithMessage("First name cannot exceed 50 characters");

        RuleFor(request => request.LastName)
            .NotEmpty().WithMessage("Last Name is required.")
            .MaximumLength(100).WithMessage("Last name cannot exceed 100 characters");

        RuleFor(request => request.Email)
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(request => request.DateOfBirth)
            .Must(date =>
            {
                var today = DateTime.Now;
                var age = today.Year - date.Year;

                return age >= 18 && age <= 120;
            }).WithMessage("Customer must be between 18 and 120 years old.");
    }
}
