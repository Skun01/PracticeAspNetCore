using System;

namespace LearnWebApi.Interfaces.Repositories;

public interface IGenericRepository<T> where T : class
{
    Task<T> AddAsync(T entity);
    Task<T> GetByIdAsync(int id);
    Task DeleteAsync(int id);
    Task UpdateAsync(T entity);
}
