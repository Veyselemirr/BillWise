using BillWise.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BillWise.Domain.Interfaces.Services
{
        public interface ICompanyService
    {        
        Task<Company> CreateCompanyAsync(string name, string taxNumber, string? address, 
            string? phone, string? email, string createdBy);
        Task<Company> UpdateCompanyAsync(int companyId, string name, string? address,
           string? phone, string? email, string updatedBy);
        Task DeactivateCompanyAsync(int companyId, string deactivatedBy);
        Task ActivateCompanyAsync(int companyId, string activatedBy);     
        Task DeleteCompanyAsync(int companyId, string deletedBy);
        Task<Company> GetCompanyByIdAsync(int companyId);
        Task<Company> GetCompanyByTaxNumberAsync(string taxNumber);
        Task<List<Company>> GetActiveCompaniesAsync();
        Task<bool> IsTaxNumberInUseAsync(string taxNumber, int? excludeCompanyId = null);
    }
}