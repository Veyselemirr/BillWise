using System;

namespace BillWise.Application.DTOs.Response.Company
{
    public class CompanyResponse
    {
        public int Id { get; set; }
         public string Name { get; set; } = string.Empty;
        public string TaxNumber { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public string? UpdatedBy { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public int UserCount { get; set; }
        public int CustomerCount { get; set; }
        public int ProductCount { get; set; }
        public int InvoiceCount { get; set; }
        public decimal MonthlyRevenue { get; set; }
        }
}