using System.ComponentModel.DataAnnotations;

namespace BillWise.Application.DTOs.Request.Company
{
    public class CreateCompanyRequest
    {  
        public string Name { get; set; } = string.Empty;     
        public string TaxNumber { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
    }
}