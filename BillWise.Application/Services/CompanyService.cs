// 📁 BillWise.Application/Services/CompanyService.cs

using AutoMapper;
using FluentValidation;
using BillWise.Application.DTOs.Request.Company;
using BillWise.Application.DTOs.Response.Company;
using BillWise.Application.Validators.Company;
using BillWise.Domain.Entities;
using BillWise.Domain.Exceptions;
using BillWise.Domain.Interfaces;
using BillWise.Domain.Interfaces.Repositories;
using BillWise.Domain.Interfaces.Services;
using ValidationException = BillWise.Domain.Exceptions.ValidationException; // Explicit using

namespace BillWise.Application.Services
{
    /// <summary>
    /// Company business logic implementation
    /// </summary>
    public class CompanyService : ICompanyService
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly CreateCompanyRequestValidator _createValidator;
        private readonly UpdateCompanyRequestValidator _updateValidator;

        public CompanyService(
            ICompanyRepository companyRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CreateCompanyRequestValidator createValidator,
            UpdateCompanyRequestValidator updateValidator)
        {
            _companyRepository = companyRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        /// <summary>
        /// Yeni şirket oluştur
        /// </summary>
        public async Task<Company> CreateCompanyAsync(string name, string taxNumber, string? address,
            string? phone, string? email, string createdBy)
        {
            // DTO oluştur
            var request = new CreateCompanyRequest
            {
                Name = name,
                TaxNumber = taxNumber,
                Address = address,
                Phone = phone,
                Email = email,
                CreatedBy = createdBy
            };

            // Validation
            var validationResult = await _createValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                throw new ValidationException("Company oluşturma validation hatası", errors);
            }

            // Vergi numarası kontrolü (eğer var ise)
            if (!string.IsNullOrEmpty(taxNumber))
            {
                var existingCompany = await _companyRepository.GetByTaxNumberAsync(taxNumber);
                if (existingCompany != null)
                {
                    throw new ValidationException("Bu vergi numarası ile kayıtlı şirket bulunmaktadır.", 
                        new List<string> { $"TaxNumber: {taxNumber} already exists" });
                }
            }

            // Entity'ye dönüştür
            var company = _mapper.Map<Company>(request);

            // Repository'ye kaydet
            await _companyRepository.AddAsync(company);
            await _unitOfWork.SaveChangesAsync();

            return company;
        }

        /// <summary>
        /// Şirket güncelle
        /// </summary>
        public async Task<Company> UpdateCompanyAsync(int companyId, string name, string? address,
            string? phone, string? email, string updatedBy)
        {
            // Mevcut company'yi bul
            var existingCompany = await _companyRepository.GetByIdAsync(companyId);
            if (existingCompany == null)
            {
                throw new NotFoundException($"ID: {companyId} olan şirket bulunamadı.");
            }

            // DTO oluştur
            var request = new UpdateCompanyRequest
            {
                Name = name,
                TaxNumber = existingCompany.TaxNumber, // TaxNumber update'te değişmez
                Address = address,
                Phone = phone,
                Email = email,
                UpdatedBy = updatedBy
            };

            // Validation
            var validationResult = await _updateValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                throw new ValidationException("Company güncelleme validation hatası", errors);
            }

            // Mapping ile güncelle
            _mapper.Map(request, existingCompany);

            // Repository'de güncelle (EF Core'da Update sync'tir)
            await _companyRepository.UpdateAsync(existingCompany);
            await _unitOfWork.SaveChangesAsync();

            return existingCompany;
        }

        /// <summary>
        /// Şirket sil (soft delete)
        /// </summary>
        public async Task DeleteCompanyAsync(int companyId, string deletedBy)
        {
            var company = await _companyRepository.GetByIdAsync(companyId);
            if (company == null)
            {
                throw new NotFoundException($"ID: {companyId} olan şirket bulunamadı.");
            }

            // Soft delete
            company.MarkAsDeleted();
            company.UpdatedBy = deletedBy;

            await _companyRepository.UpdateAsync(company);
            await _unitOfWork.SaveChangesAsync();
        }

        /// <summary>
        /// ID ile şirket getir
        /// </summary>
        public async Task<Company> GetCompanyByIdAsync(int companyId)
        {
            var company = await _companyRepository.GetByIdAsync(companyId);
            if (company == null)
            {
                throw new NotFoundException($"ID: {companyId} olan şirket bulunamadı.");
            }

            return company;
        }

        /// <summary>
        /// Vergi numarası ile şirket getir
        /// </summary>
        public async Task<Company> GetCompanyByTaxNumberAsync(string taxNumber)
        {
            if (string.IsNullOrEmpty(taxNumber))
            {
                throw new ValidationException("Vergi numarası boş olamaz.", 
                    new List<string> { "TaxNumber is required" });
            }

            var company = await _companyRepository.GetByTaxNumberAsync(taxNumber);
            if (company == null)
            {
                throw new NotFoundException($"Vergi numarası: {taxNumber} olan şirket bulunamadı.");
            }

            return company;
        }

        /// <summary>
        /// Aktif şirketleri getir
        /// </summary>
        public async Task<List<Company>> GetActiveCompaniesAsync()
        {
            return await _companyRepository.GetActiveCompaniesAsync();
        }

        /// <summary>
        /// Şirketi aktif hale getirir
        /// </summary>
        public async Task ActivateCompanyAsync(int companyId, string activatedBy)
        {
            var company = await _companyRepository.GetByIdAsync(companyId);
            if (company == null)
            {
                throw new NotFoundException($"ID: {companyId} olan şirket bulunamadı.");
            }

            company.Activate();
            company.UpdatedBy = activatedBy;

            await _companyRepository.UpdateAsync(company);
            await _unitOfWork.SaveChangesAsync();
        }

        /// <summary>
        /// Şirketi deaktif hale getirir
        /// </summary>
        public async Task DeactivateCompanyAsync(int companyId, string deactivatedBy)
        {
            var company = await _companyRepository.GetByIdAsync(companyId);
            if (company == null)
            {
                throw new NotFoundException($"ID: {companyId} olan şirket bulunamadı.");
            }

            company.Deactivate();
            company.UpdatedBy = deactivatedBy;

            await _companyRepository.UpdateAsync(company);
            await _unitOfWork.SaveChangesAsync();
        }

        /// <summary>
        /// Vergi numarasının kullanımda olup olmadığını kontrol eder
        /// </summary>
        public async Task<bool> IsTaxNumberInUseAsync(string taxNumber, int? excludeCompanyId = null)
        {
            if (string.IsNullOrWhiteSpace(taxNumber))
            {
                return false;
            }

            var existingCompany = await _companyRepository.GetByTaxNumberAsync(taxNumber);

            if (existingCompany == null)
            {
                return false;
            }

            // Eğer excludeCompanyId varsa ve bulunan şirket bu ID ise, kullanımda sayma
            if (excludeCompanyId.HasValue && existingCompany.Id == excludeCompanyId.Value)
            {
                return false;
            }

            return true;
        }
    }
}