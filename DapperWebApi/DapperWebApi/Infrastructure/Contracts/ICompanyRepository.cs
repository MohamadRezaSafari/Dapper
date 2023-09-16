using DapperWebApi.Domain;
using DapperWebApi.Dto;

namespace DapperWebApi.Infrastructure.Contracts;

public interface ICompanyRepository
{
    Task CreateMultipleCompanies(List<CreateCompanyDto> companyDtos);
    Task<List<Company>> GetMultipleMapping();
    Task<Company> GetMultipleResults(int id);
    Task<Company> SP_GetCompany(int id);
    Task DeleteCompany(int id);
    Task UpdateCompany(int id, UpdateCompanyDto companyDto);
    Task CreateCompany(CreateCompanyDto companyDto);
    Task<IEnumerable<Company>> GetCompanies();
    Task<Company> GetCompany(int id);
}