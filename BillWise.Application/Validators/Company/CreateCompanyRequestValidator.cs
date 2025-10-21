// 📁 BillWise.Application/Validators/Company/CreateCompanyRequestValidator.cs

using BillWise.Application.DTOs.Request.Company;
using FluentValidation;

namespace BillWise.Application.Validators.Company
{
    /// <summary>
    /// CreateCompanyRequest için validation kuralları
    /// Single Responsibility: Sadece validation logic
    /// </summary>
    public class CreateCompanyRequestValidator : AbstractValidator<CreateCompanyRequest>
    {
        public CreateCompanyRequestValidator()
        {
            // ========== NAME VALIDATION ==========
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Şirket adı zorunludur")
                .Length(2, 100)
                .WithMessage("Şirket adı 2-100 karakter arası olmalıdır")
                .Must(BeValidCompanyName)
                .WithMessage("Şirket adı geçersiz karakterler içeriyor");

            // ========== TAX NUMBER VALIDATION ==========
            RuleFor(x => x.TaxNumber)
                .NotEmpty()
                .WithMessage("Vergi numarası zorunludur")
                .Length(10, 11)
                .WithMessage("Vergi numarası 10-11 karakter olmalıdır")
                .Matches(@"^\d{10,11}$")
                .WithMessage("Vergi numarası sadece rakamlardan oluşmalıdır")
                .Must(BeValidTaxNumber)
                .WithMessage("Geçersiz vergi numarası formatı");

            // ========== ADDRESS VALIDATION ==========
            RuleFor(x => x.Address)
                .MaximumLength(500)
                .WithMessage("Adres en fazla 500 karakter olabilir")
                .When(x => !string.IsNullOrEmpty(x.Address));

            // ========== PHONE VALIDATION ==========
            RuleFor(x => x.Phone)
                .MaximumLength(20)
                .WithMessage("Telefon numarası en fazla 20 karakter olabilir")
                .Matches(@"^[\d\s\-\(\)\+]+$")
                .WithMessage("Geçersiz telefon formatı")
                .When(x => !string.IsNullOrEmpty(x.Phone));

            // ========== EMAIL VALIDATION ==========
            RuleFor(x => x.Email)
                .MaximumLength(100)
                .WithMessage("Email en fazla 100 karakter olabilir")
                .EmailAddress()
                .WithMessage("Geçersiz email formatı")
                .When(x => !string.IsNullOrEmpty(x.Email));

            // ========== CREATED BY VALIDATION ==========
          /*  RuleFor(x => x.CreatedBy)
                .NotEmpty()
                .WithMessage("Oluşturan kullanıcı bilgisi zorunludur")
                .EmailAddress()
                .WithMessage("Oluşturan kullanıcı geçerli email formatında olmalıdır");*/
        }

        // ========== CUSTOM VALIDATION METHODS ==========

        /// <summary>
        /// Şirket adının geçerli olup olmadığını kontrol eder
        /// </summary>
        private bool BeValidCompanyName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            // Başında/sonunda boşluk olmasın
            if (name != name.Trim())
                return false;

            // Sadece harf, rakam, boşluk ve bazı özel karakterlere izin ver
            return System.Text.RegularExpressions.Regex.IsMatch(name, @"^[a-zA-ZğüşıöçĞÜŞIÖÇ0-9\s\.\-\&]+$");
        }

        /// <summary>
        /// Vergi numarasının geçerli olup olmadığını kontrol eder
        /// Basit format kontrolü (gerçek projede daha detaylı olabilir)
        /// </summary>
        private bool BeValidTaxNumber(string taxNumber)
        {
            if (string.IsNullOrWhiteSpace(taxNumber))
                return false;

            // Sadece rakam kontrolü (zaten Regex ile yapılıyor ama ek kontrol)
            if (!taxNumber.All(char.IsDigit))
                return false;

            // 10 haneli için basit checksum kontrolü yapılabilir
            // 11 haneli TC kimlik için mod 11 kontrolü yapılabilir
            // Şimdilik basit format kontrolü yapıyoruz

            return taxNumber.Length >= 10 && taxNumber.Length <= 11;
        }
    }
}