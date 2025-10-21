using Microsoft.EntityFrameworkCore;
using BillWise.Domain.Entities.Common;
using BillWise.Domain.Interfaces;
using System.Linq.Expressions;

namespace BillWise.Infrastructure.Data.Repositories
{
    /// <summary>
    /// Generic Repository implementasyonu
    /// Tüm entity'ler için ortak CRUD operasyonlarını sağlar
    /// </summary>
    /// <typeparam name="T">BaseEntity'den türeyen entity tipi</typeparam>
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        protected readonly BillWiseDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(BillWiseDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        // ========== QUERY METHODS ==========

        public virtual async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task<List<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public virtual async Task<List<T>> GetActiveAsync()
        {
            // IsDeleted = false olan kayıtları getir (Global Query Filter zaten var)
            return await _dbSet.ToListAsync();
        }

        public virtual async Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.FirstOrDefaultAsync(predicate);
        }

        public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }

        public virtual async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null)
        {
            if (predicate == null)
            {
                return await _dbSet.CountAsync();
            }
            return await _dbSet.CountAsync(predicate);
        }

        public virtual async Task<(List<T> Items, int TotalCount)> GetPagedAsync(
            int pageNumber,
            int pageSize,
            Expression<Func<T, bool>>? predicate = null)
        {
            var query = _dbSet.AsQueryable();

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        // ========== COMMAND METHODS ==========

        public virtual async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            return entity;
        }

        public virtual async Task<List<T>> AddRangeAsync(List<T> entities)
        {
            await _dbSet.AddRangeAsync(entities);
            return entities;
        }

        public virtual async Task<T> UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            return await Task.FromResult(entity);
        }

        public virtual async Task<T> DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity == null)
            {
                throw new InvalidOperationException($"Entity with ID {id} not found");
            }

            // Soft delete
            entity.MarkAsDeleted();
            _dbSet.Update(entity);

            return entity;
        }

        public virtual async Task<T> DeleteAsync(T entity)
        {
            // Soft delete
            entity.MarkAsDeleted();
            _dbSet.Update(entity);

            return await Task.FromResult(entity);
        }
    }
}
