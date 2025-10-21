using BillWise.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BillWise.Domain.Interfaces
{
    public interface IRepository<T> where T : BaseEntity
    {
        Task<T?> GetByIdAsync(int id);
        Task<List<T>> GetAllAsync();
        Task<List<T>> GetActiveAsync();
        Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
        Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);
        Task<T> AddAsync(T entity);
        Task<List<T>> AddRangeAsync(List<T> entities);
        Task<T> UpdateAsync(T entity);
        Task<T> DeleteAsync(int id);
        Task<T> DeleteAsync(T entity);
        Task<(List<T> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, 
            Expression<Func<T, bool>>? predicate = null);
    }
}