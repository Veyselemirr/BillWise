using Microsoft.EntityFrameworkCore;
using BillWise.Domain.Entities;
using BillWise.Domain.Interfaces.Repositories;

namespace BillWise.Infrastructure.Data.Repositories
{
    /// <summary>
    /// Company-specific repository implementasyonu
    /// ICompanyRepository interface'ini implement eder
    /// </summary>
    public class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        public CompanyRepository(BillWiseDbContext context) : base(context)
        {
        }

        // ========== COMPANY-SPECIFIC METHODS ==========

        public async Task<Company?> GetByTaxNumberAsync(string taxNumber)
        {
            return await _dbSet
                .FirstOrDefaultAsync(c => c.TaxNumber == taxNumber);
        }

        public async Task<List<Company>> GetActiveCompaniesAsync()
        {
            return await _dbSet
                .Where(c => c.IsActive && !c.IsDeleted)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<List<Company>> SearchByNameAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return new List<Company>();
            }

            return await _dbSet
                .Where(c => c.Name.Contains(searchTerm))
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<bool> IsTaxNumberExistsAsync(string taxNumber, int? excludeId = null)
        {
            var query = _dbSet.Where(c => c.TaxNumber == taxNumber);

            if (excludeId.HasValue)
            {
                query = query.Where(c => c.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<bool> CanBeDeletedAsync(int companyId)
        {
            var company = await _dbSet
                .Include(c => c.Users)
                .Include(c => c.Customers)
                .Include(c => c.Products)
                .Include(c => c.Invoices)
                .FirstOrDefaultAsync(c => c.Id == companyId);

            if (company == null)
            {
                return false;
            }

            // Şirketin kullanıcısı, müşterisi, ürünü veya faturası varsa silinemez
            return !company.Users.Any() &&
                   !company.Customers.Any() &&
                   !company.Products.Any() &&
                   !company.Invoices.Any();
        }

        // ========== OVERRIDES WITH EAGER LOADING ==========

        /// <summary>
        /// ID ile company getir (ilişkili verilerle birlikte)
        /// </summary>
        public async Task<Company?> GetByIdWithRelationsAsync(int id)
        {
            return await _dbSet
                .Include(c => c.Users)
                .Include(c => c.Customers)
                .Include(c => c.Products)
                .Include(c => c.Invoices)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        /// <summary>
        /// Pagination ile company listesi (ilişki sayıları ile)
        /// </summary>
        public async Task<List<Company>> GetAllAsync(int page, int pageSize)
        {
            return await _dbSet
                .OrderBy(c => c.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
    }
}
