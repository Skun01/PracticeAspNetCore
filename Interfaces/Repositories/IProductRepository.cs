using System;
using LearnWebApi.DTOs;
using LearnWebApi.Entities;

namespace LearnWebApi.Interfaces.Repositories;

public interface IProductRepository : IGenericRepository<Product>
{
    Task<IEnumerable<Product>> SearchProductAsync(ProductQueryParameters query);
    Task<IEnumerable<Product>> GetAll();
}
