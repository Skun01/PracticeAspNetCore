using System;
using LearnWebApi.Data;
using LearnWebApi.DTOs;
using LearnWebApi.DTOs.Product;
using LearnWebApi.Entities;
using LearnWebApi.Shared;

namespace LearnWebApi.Interfaces;

public interface IProductService
{
    Task<Result<Product>> GetProductByIdAsync(int productId);
    Task<PageList<Product>> SearchProducts(ProductQueryParameters query);
    Task<Result<Product>> CreateProductAsync(ProductRequest request);
}
