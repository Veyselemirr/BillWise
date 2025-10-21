using System;

namespace BillWise.Application.DTOs.Response.Company
{
        public class CompanyListResponse
    {
        
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string TaxNumber { get; set; } = string.Empty;
        public string? City { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public int CustomerCount { get; set; }
        public int MonthlyInvoiceCount { get; set; }
        public string StatusText => IsActive ? "Aktif" : "Pasif";
    }
}