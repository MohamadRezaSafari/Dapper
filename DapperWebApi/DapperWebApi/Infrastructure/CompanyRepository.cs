using System.Data;
using Dapper;
using DapperWebApi.Domain;
using DapperWebApi.Dto;
using DapperWebApi.Infrastructure.Context;
using DapperWebApi.Infrastructure.Contracts;

namespace DapperWebApi.Infrastructure;

public class CompanyRepository : ICompanyRepository
{
    private readonly DapperContext _context;

    public CompanyRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task CreateMultipleCompanies(List<CreateCompanyDto> companyDtos)
    {
        var query = "insert into companies (Name, Address, Country) values (@Name, @Address, @Country)";

        using var connection = _context.CreateConnection();
        connection.Open();

        using var transaction = connection.BeginTransaction();
        foreach (var company in companyDtos)
        {
            var parameters = new DynamicParameters();
            parameters.Add(nameof(Company.Name), company.Name, DbType.String);
            parameters.Add(nameof(Company.Address), company.Address, DbType.String);
            parameters.Add(nameof(Company.Country), company.Country, DbType.String);

            await connection.ExecuteAsync(query, parameters, transaction: transaction);
        }

        transaction.Commit();
    }

    public async Task<List<Company>> GetMultipleMapping()
    {
        var query = "select * from companies c join employees e on c.Id = e.CompanyId";

        using var connection = _context.CreateConnection();
        var companyDict = new Dictionary<int, Company>();

        var companies = await connection
            .QueryAsync<Company, Employee, Company>(
                query, (company, employee) =>
                {
                    if (!companyDict.TryGetValue(company.Id, out var currentCompany))
                    {
                        currentCompany = company;
                        companyDict.Add(currentCompany.Id, currentCompany);
                    }

                    currentCompany.Employees.Add(employee);

                    return currentCompany;
                });

        return companies.Distinct().ToList();
    }

    public async Task<Company> GetMultipleResults(int id)
    {
        var query = "select * from companies where Id = @Id;"
                    + "select * from employees where Id = @Id";

        using var connection = _context.CreateConnection();
        using var multi = await connection.QueryMultipleAsync(query, new { id });
        var company = await multi.ReadSingleOrDefaultAsync<Company>();

        if (company is not null)
            company.Employees = (await multi.ReadAsync<Employee>()).ToList();

        return company;
    }

    public async Task<Company> SP_GetCompany(int id)
    {
        var procedureName = "GetCompany";
        var parameters = new DynamicParameters();
        parameters.Add(nameof(Company.Id), id, DbType.Int32, ParameterDirection.Input);

        using var connection = _context.CreateConnection();
        var company = await connection.QueryFirstOrDefaultAsync<Company>
            (procedureName, parameters, commandType: CommandType.StoredProcedure);

        return company;
    }

    public async Task DeleteCompany(int id)
    {
        var query = "delete from companies where Id = @Id";

        using var connection = _context.CreateConnection();
        await connection.ExecuteAsync(query, new { id });
    }

    public async Task UpdateCompany(int id, UpdateCompanyDto companyDto)
    {
        var query = "update companies set Name = @Name, Address = @Address, Country = @Country where Id = @Id";

        var parameters = new DynamicParameters();
        parameters.Add(nameof(Company.Id), id, DbType.Int32);
        parameters.Add(nameof(Company.Name), companyDto.Name, DbType.String);
        parameters.Add(nameof(Company.Address), companyDto.Address, DbType.String);
        parameters.Add(nameof(Company.Country), companyDto.Country, DbType.String);

        using var connection = _context.CreateConnection();
        await connection.ExecuteAsync(query, parameters);
    }

    public async Task CreateCompany(CreateCompanyDto companyDto)
    {
        var query = "insert into companies (Name, Address, Country) values (@Name, @Address, @Country)";
        //  + select cast(SCOPE_IDENTITY() as int)

        var parameters = new DynamicParameters();
        parameters.Add(nameof(Company.Name), companyDto.Name, DbType.String);
        parameters.Add(nameof(Company.Address), companyDto.Address, DbType.String);
        parameters.Add(nameof(Company.Country), companyDto.Country, DbType.String);

        using var connection = _context.CreateConnection();
        await connection.ExecuteAsync(query, parameters);
        // var id = await connection.QuerySingleAsync<int>(query, parameters);
    }

    public async Task<IEnumerable<Company>> GetCompanies()
    {
        var query = "select * from companies";

        using var connection = _context.CreateConnection();
        var companies = await connection.QueryAsync<Company>(query);

        return companies;
    }

    public async Task<Company> GetCompany(int id)
    {
        var query = "select * from companies where Id = @Id";

        using var connection = _context.CreateConnection();
        var company = await connection.QuerySingleOrDefaultAsync<Company>(query, new { id });

        return company;
    }
}