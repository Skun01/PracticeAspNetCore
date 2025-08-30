using LearnWebApi.DTOs;
using LearnWebApi.DTOs.Product;
using LearnWebApi.Entities;
using LearnWebApi.Interfaces;
using LearnWebApi.Interfaces.Repositories;
using LearnWebApi.Shared;

namespace LearnWebApi.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<Result<Product>> CreateProductAsync(ProductRequest request)
    {
        Product newProduct = new()
        {
            Name = request.Name,
            Price = request.Price
        };
        await _productRepository.AddAsync(newProduct);
        return Result.Success(newProduct);
    }

    public async Task<Result<Product>> GetProductByIdAsync(int productId)
    {
        try
        {
            var product = await _productRepository.GetByIdAsync(productId);
            return Result.Success(product);
        }
        catch (KeyNotFoundException ex)
        {
            return Result.Failure<Product>(new Error("404", ex.Message));
        }
    }

    public async Task<PageList<Product>> SearchProducts(ProductQueryParameters query)
    {
        var searchResult = await _productRepository.SearchProductAsync(query);
        return new PageList<Product>(searchResult.ToList(), query.PageNumer, query.PageSize);
    }
}
