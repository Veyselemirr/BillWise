using AutoMapper;
using BillWise.Application.DTOs.Request;
using BillWise.Application.DTOs.Request.Company;
using BillWise.Application.DTOs.Response.Company;
using BillWise.Domain.Entities;

namespace BillWise.Application.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {           
            CreateMap<CreateCompanyRequest, Company>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) // ID database tarafından verilecek
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true)) // Yeni şirketler aktif
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore()) // İlk oluşturmada null
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore()) // İlk oluşturmada null
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => false))
                // Navigation properties ignore edilir
                .ForMember(dest => dest.Users, opt => opt.Ignore())
                .ForMember(dest => dest.Customers, opt => opt.Ignore())
                .ForMember(dest => dest.Invoices, opt => opt.Ignore())
                .ForMember(dest => dest.Products, opt => opt.Ignore());

            // UpdateCompanyRequest + Company Entity → Company Entity (Güncelleme için)
            CreateMap<UpdateCompanyRequest, Company>()
                .ForMember(dest => dest.TaxNumber, opt => opt.Ignore()) // Vergi numarası değiştirilemez
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()) // Oluşturulma tarihi değişmez
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore()) // Oluşturan değişmez
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore()) // Soft delete ayrı işlem
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                // Navigation properties ignore edilir
                .ForMember(dest => dest.Users, opt => opt.Ignore())
                .ForMember(dest => dest.Customers, opt => opt.Ignore())
                .ForMember(dest => dest.Invoices, opt => opt.Ignore())
                .ForMember(dest => dest.Products, opt => opt.Ignore());

            // Company Entity → CompanyResponse
            CreateMap<Company, CompanyResponse>()
                .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.DisplayName))
                // Navigation property counts - bu bilgiler Service'de hesaplanacak ve manuel set edilecek
                .ForMember(dest => dest.UserCount, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerCount, opt => opt.Ignore())
                .ForMember(dest => dest.ProductCount, opt => opt.Ignore())
                .ForMember(dest => dest.InvoiceCount, opt => opt.Ignore())
                .ForMember(dest => dest.MonthlyRevenue, opt => opt.Ignore());

            // Company Entity → CompanyListResponse (Liste için optimize edilmiş)
            CreateMap<Company, CompanyListResponse>()
                .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.DisplayName))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => ExtractCityFromAddress(src.Address)))
                // Liste için istatistikler - Service'de hesaplanacak
                .ForMember(dest => dest.CustomerCount, opt => opt.Ignore())
                .ForMember(dest => dest.MonthlyInvoiceCount, opt => opt.Ignore());

            // ========== GELECEKTE EKLENECEKlER ==========
            // User mappings...
            // Customer mappings...
            // Product mappings...
            // Invoice mappings...
        }

        // ========== HELPER METHODS ==========

        /// <summary>
        /// Adres string'inden şehir bilgisini çıkarır
        /// Basit yaklaşım - gerçek projede daha sofistike olabilir
        /// </summary>
        private static string? ExtractCityFromAddress(string? address)
        {
            if (string.IsNullOrWhiteSpace(address))
                return null;

            // Basit yaklaşım: Son kelimeyi şehir olarak kabul et
            // Gerçek projede daha karmaşık parsing yapılabilir
            var parts = address.Split(',', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length > 0)
            {
                return parts[^1].Trim(); // Son parçayı al
            }

            return null;
        }
    }
}