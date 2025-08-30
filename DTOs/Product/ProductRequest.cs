using System;
using System.ComponentModel.DataAnnotations;

namespace LearnWebApi.DTOs.Product;

public record ProductRequest(string Name, decimal Price);
