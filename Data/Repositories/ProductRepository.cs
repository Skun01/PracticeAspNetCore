using System;
using LearnWebApi.DTOs;
using LearnWebApi.Entities;
using LearnWebApi.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LearnWebApi.Data.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ProjectContext _context;
    public ProductRepository(ProjectContext context)
    {
        _context = context;
    }
    public async Task<Product> AddAsync(Product entity)
    {
        _context.Products.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public Task DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<Product> GetByIdAsync(int id)
    {
        Product? target = await _context.Products.FindAsync(id);
        if (target is null)
            throw new KeyNotFoundException($"Product not found!");

        return target;
    }

    public async Task<IEnumerable<Product>> SearchProductAsync(ProductQueryParameters query)
    {
        IQueryable<Product> searchQuery = _context.Products.AsQueryable();

        if (!string.IsNullOrEmpty(query.Name))
            searchQuery = searchQuery.Where(p => p.Name!.Contains(query.Name));

        if (query.MinPrice.HasValue)
            searchQuery = searchQuery.Where(p => p.Price >= query.MinPrice);

        if (query.MaxPrice.HasValue)
            searchQuery = searchQuery.Where(p => p.Price <= query.MaxPrice);

        if (!string.IsNullOrEmpty(query.SortBy))
        {
            searchQuery = query.SortBy.ToLower() switch
            {
                "name" => searchQuery.OrderBy(p => p.Name),
                "price" => searchQuery.OrderBy(p => p.Price),
                _ => throw new ArgumentException($"Value in sortBy does not valid!")
            };
        }

        List<Product> products = await searchQuery
            .Skip((query.PageNumer - 1) * query.PageSize)
            .Take(query.PageSize)
            .AsNoTracking()
            .ToListAsync();

        return products;
    }

    public Task UpdateAsync(Product entity)
    {
        throw new NotImplementedException();
    }
}
