namespace BillWise.Application.DTOs.Request.Company
{
    public class UpdateCompanyRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? TaxNumber { get; set; }  // Sadece mapper için, güncellemede kullanılmaz
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string UpdatedBy { get; set; } = string.Empty;
    }
}
