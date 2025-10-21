
using BillWise.Application.DTOs.Request;
using BillWise.Application.DTOs.Request.Company;
using FluentValidation;
using System.Linq;

namespace BillWise.Application.Validators.Company
{
    /// <summary>
    /// UpdateCompanyRequest için validation kuralları
    /// Single Responsibility: Sadece validation logic
    /// </summary>
    public class UpdateCompanyRequestValidator : AbstractValidator<UpdateCompanyRequest>
    {
        public UpdateCompanyRequestValidator()
        {
            // ========== NAME VALIDATION ==========
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Şirket adı zorunludur")
                .Length(2, 100)
                .WithMessage("Şirket adı 2-100 karakter arası olmalıdır")
                .Must(BeValidCompanyName)
                .WithMessage("Şirket adı geçersiz karakterler içeriyor");

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

            // ========== UPDATED BY VALIDATION ==========
            RuleFor(x => x.UpdatedBy)
                .NotEmpty()
                .WithMessage("Güncelleyen kullanıcı bilgisi zorunludur")
                .MaximumLength(100)
                .WithMessage("Güncelleyen kullanıcı bilgisi en fazla 100 karakter olabilir");
        }

        // ========== CUSTOM VALIDATION METHODS ==========

        /// <summary>
        /// Şirket adının geçerli olup olmadığını kontrol eder
        /// CreateCompanyRequestValidator ile aynı logic
        /// (Ortak logic için base class veya utility method kullanılabilir)
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
    }
}