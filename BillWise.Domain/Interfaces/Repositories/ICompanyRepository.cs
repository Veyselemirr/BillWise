using BillWise.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BillWise.Domain.Interfaces.Repositories
{
    public interface ICompanyRepository : IRepository<Company>
    {
        Task<Company?> GetByTaxNumberAsync(string taxNumber);
        Task<List<Company>> GetActiveCompaniesAsync();
        Task<List<Company>> SearchByNameAsync(string searchTerm);
        Task<bool> IsTaxNumberExistsAsync(string taxNumber, int? excludeId = null);
        Task<bool> CanBeDeletedAsync(int companyId);
    }
}