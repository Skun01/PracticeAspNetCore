using LearnWebApi.DTOs;
using LearnWebApi.DTOs.Product;
using LearnWebApi.Entities;
using LearnWebApi.Interfaces;
using LearnWebApi.Interfaces.Repositories;
using LearnWebApi.Interfaces.Services;
using LearnWebApi.Shared;
using Microsoft.Extensions.Caching.Memory;
using Serilog;

namespace LearnWebApi.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly ICacheService _cache;
    public ProductService(IProductRepository productRepository, ICacheService cache)
    {
        _productRepository = productRepository;
        _cache = cache;
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
            string cacheKey = $"book_{productId}";
            Product? product = _cache.Get<Product>(cacheKey);
            if (product is not null)
                return Result.Success(product);

            product = await _productRepository.GetByIdAsync(productId);
            _cache.Set(cacheKey, product, TimeSpan.FromSeconds(60) ,sliding: true);

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

    public async Task<IEnumerable<Product>> GetAllProducts()
    {
        string cacheKey = "books";
        IEnumerable<Product>? products = _cache.Get<IEnumerable<Product>>(cacheKey);
        if (products is not null)
        {
            return products;
        }
        Log.Information($"Cache MISS for {cacheKey}. Querying DB");
        products = await _productRepository.GetAll();
        _cache.Set(cacheKey, products, TimeSpan.FromSeconds(60), sliding: true);
        return products;

    }
}
