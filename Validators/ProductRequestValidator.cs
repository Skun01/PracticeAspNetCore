using System;
using FluentValidation;
using LearnWebApi.DTOs.Product;

namespace LearnWebApi.Validators;

public class ProductRequestValidator : AbstractValidator<ProductRequest>
{
    public ProductRequestValidator()
    {
        RuleFor(p => p.Name)
            .NotEmpty().WithMessage("Product name must be required")
            .MaximumLength(50).WithMessage("Product Name must have length within 50 characters");

        RuleFor(p => p.Price)
            .NotNull().WithMessage("Price must be required")
            .GreaterThan(0).WithMessage("Price must be greater than 0");
    }
}
