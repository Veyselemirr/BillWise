using Microsoft.AspNetCore.Mvc;
using BillWise.Domain.Interfaces.Services;
using BillWise.Domain.Exceptions;
using BillWise.Application.DTOs.Response.Company;
using AutoMapper;

namespace BillWise.API.Controllers
{
    /// <summary>
    /// Company management endpoints
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyService _companyService;
        private readonly IMapper _mapper;
        private readonly ILogger<CompanyController> _logger;

        public CompanyController(
            ICompanyService companyService,
            IMapper mapper,
            ILogger<CompanyController> logger)
        {
            _companyService = companyService;
            _mapper = mapper;
            _logger = logger;
        }

        // ========== CREATE ==========

        /// <summary>
        /// Yeni şirket oluşturur
        /// </summary>
        /// <param name="request">Şirket bilgileri</param>
        /// <returns>Oluşturulan şirket</returns>
        [HttpPost]
        [ProducesResponseType(typeof(CompanyResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CompanyResponse>> CreateCompany([FromBody] CreateCompanyDto request)
        {
            try
            {
                var company = await _companyService.CreateCompanyAsync(
                    name: request.Name,
                    taxNumber: request.TaxNumber,
                    address: request.Address,
                    phone: request.Phone,
                    email: request.Email,
                    createdBy: request.CreatedBy ?? "system@billwise.com" // TODO: Get from auth user
                );

                var response = _mapper.Map<CompanyResponse>(company);

                _logger.LogInformation("Company created: {CompanyId} - {CompanyName}", company.Id, company.Name);

                return CreatedAtAction(
                    nameof(GetCompanyById),
                    new { id = company.Id },
                    response
                );
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning("Validation failed: {Errors}", ex.GetAllErrors());
                return BadRequest(new { errors = ex.Errors });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating company");
                return StatusCode(500, new { message = "Şirket oluşturulurken bir hata oluştu" });
            }
        }

        // ========== READ ==========

        /// <summary>
        /// ID ile şirket getirir
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CompanyResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CompanyResponse>> GetCompanyById(int id)
        {
            try
            {
                var company = await _companyService.GetCompanyByIdAsync(id);
                var response = _mapper.Map<CompanyResponse>(company);
                return Ok(response);
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning("Company not found: {CompanyId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting company {CompanyId}", id);
                return StatusCode(500, new { message = "Şirket getirilirken bir hata oluştu" });
            }
        }

        /// <summary>
        /// Vergi numarası ile şirket getirir
        /// </summary>
        [HttpGet("by-tax-number/{taxNumber}")]
        [ProducesResponseType(typeof(CompanyResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CompanyResponse>> GetCompanyByTaxNumber(string taxNumber)
        {
            try
            {
                var company = await _companyService.GetCompanyByTaxNumberAsync(taxNumber);
                var response = _mapper.Map<CompanyResponse>(company);
                return Ok(response);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { errors = ex.GetAllErrors() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting company by tax number {TaxNumber}", taxNumber);
                return StatusCode(500, new { message = "Şirket getirilirken bir hata oluştu" });
            }
        }

        /// <summary>
        /// Aktif şirketleri listeler
        /// </summary>
        [HttpGet("active")]
        [ProducesResponseType(typeof(List<CompanyListResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<CompanyListResponse>>> GetActiveCompanies()
        {
            try
            {
                var companies = await _companyService.GetActiveCompaniesAsync();
                var response = _mapper.Map<List<CompanyListResponse>>(companies);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active companies");
                return StatusCode(500, new { message = "Şirketler getirilirken bir hata oluştu" });
            }
        }

        // ========== UPDATE ==========

        /// <summary>
        /// Şirket günceller
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(CompanyResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CompanyResponse>> UpdateCompany(int id, [FromBody] UpdateCompanyDto request)
        {
            try
            {
                var company = await _companyService.UpdateCompanyAsync(
                    companyId: id,
                    name: request.Name,
                    address: request.Address,
                    phone: request.Phone,
                    email: request.Email,
                    updatedBy: request.UpdatedBy ?? "system@billwise.com" // TODO: Get from auth user
                );

                var response = _mapper.Map<CompanyResponse>(company);

                _logger.LogInformation("Company updated: {CompanyId} - {CompanyName}", company.Id, company.Name);

                return Ok(response);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { errors = ex.Errors });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating company {CompanyId}", id);
                return StatusCode(500, new { message = "Şirket güncellenirken bir hata oluştu" });
            }
        }

        /// <summary>
        /// Şirketi aktif eder
        /// </summary>
        [HttpPost("{id}/activate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ActivateCompany(int id)
        {
            try
            {
                await _companyService.ActivateCompanyAsync(id, "system@billwise.com"); // TODO: Auth user
                _logger.LogInformation("Company activated: {CompanyId}", id);
                return Ok(new { message = "Şirket aktif edildi" });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error activating company {CompanyId}", id);
                return StatusCode(500, new { message = "Şirket aktif edilirken bir hata oluştu" });
            }
        }

        /// <summary>
        /// Şirketi pasif eder
        /// </summary>
        [HttpPost("{id}/deactivate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeactivateCompany(int id)
        {
            try
            {
                await _companyService.DeactivateCompanyAsync(id, "system@billwise.com"); // TODO: Auth user
                _logger.LogInformation("Company deactivated: {CompanyId}", id);
                return Ok(new { message = "Şirket pasif edildi" });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deactivating company {CompanyId}", id);
                return StatusCode(500, new { message = "Şirket pasif edilirken bir hata oluştu" });
            }
        }

        // ========== DELETE ==========

        /// <summary>
        /// Şirketi siler (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteCompany(int id)
        {
            try
            {
                await _companyService.DeleteCompanyAsync(id, "system@billwise.com"); // TODO: Auth user
                _logger.LogInformation("Company deleted: {CompanyId}", id);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting company {CompanyId}", id);
                return StatusCode(500, new { message = "Şirket silinirken bir hata oluştu" });
            }
        }

        // ========== UTILITY ==========

        /// <summary>
        /// Vergi numarasının kullanımda olup olmadığını kontrol eder
        /// </summary>
        [HttpGet("check-tax-number/{taxNumber}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> CheckTaxNumberInUse(string taxNumber, [FromQuery] int? excludeCompanyId = null)
        {
            try
            {
                var inUse = await _companyService.IsTaxNumberInUseAsync(taxNumber, excludeCompanyId);
                return Ok(new { taxNumber, inUse });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking tax number {TaxNumber}", taxNumber);
                return StatusCode(500, new { message = "Vergi numarası kontrolü yapılırken bir hata oluştu" });
            }
        }
    }

    // ========== DTOs ==========

    /// <summary>
    /// Yeni şirket oluşturma request DTO
    /// </summary>
    public record CreateCompanyDto(
        string Name,
        string TaxNumber,
        string? Address,
        string? Phone,
        string? Email,
        string? CreatedBy
    );

    /// <summary>
    /// Şirket güncelleme request DTO
    /// </summary>
    public record UpdateCompanyDto(
        string Name,
        string? Address,
        string? Phone,
        string? Email,
        string? UpdatedBy
    );
}
