using DapperWebApi.Dto;
using DapperWebApi.Infrastructure.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace DapperWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CompaniesController : ControllerBase
    {
        private readonly ICompanyRepository _companyRepo;

        public CompaniesController(ICompanyRepository companyRepo)
        {
            _companyRepo = companyRepo;
        }

        [HttpGet("MultipleMapping")]
        public async Task<IActionResult> GetMultipleMapping()
        {
            var companies = await _companyRepo.GetMultipleMapping();

            return Ok(companies);
        }

        [HttpGet]
        public async Task<IActionResult> GetMultipleResults(int id)
        {
            var company = await _companyRepo.GetMultipleResults(id);

            if (company is null)
                return NotFound();

            return Ok(company);
        }

        [HttpGet("SP_GetCompany")]
        public async Task<IActionResult> SP_GetCompany(int id)
        {
            var company = await _companyRepo.SP_GetCompany(id);

            if (company is null)
                return NotFound();

            return Ok(company);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompany(int id)
        {
            var company = await _companyRepo.GetCompany(id);

            if (company is null)
                return NotFound();

            await _companyRepo.DeleteCompany(id);

            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCompany(int id,
            [FromBody] UpdateCompanyDto companyDto)
        {
            var company = await _companyRepo.GetCompany(id);

            if (company is null)
                return NotFound();

            await _companyRepo.UpdateCompany(id, companyDto);

            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> CreateCompany(
            [FromBody] CreateCompanyDto companyDto)
        {
            await _companyRepo.CreateCompany(companyDto);

            return Ok("Done!");
        }

        [HttpGet]
        public async Task<IActionResult> GetCompanies()
        {
            var companies = _companyRepo.GetCompanies();

            return Ok(companies);
        }

        [HttpGet("{id}", Name = "CompanyById")]
        public async Task<IActionResult> GetCompany(int id)
        {
            var company = _companyRepo.GetCompany(id);

            if (company is null)
                return NotFound();

            return Ok(company);
        }
    }
}