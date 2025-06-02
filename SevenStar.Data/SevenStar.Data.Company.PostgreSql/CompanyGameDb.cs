using Common.Enums;
using Infrastructure.Data.PostgreSql;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using SevenStar.Shared.Domain.DbContext.Company;
using SevenStar.Shared.Domain.Service;

namespace SevenStar.Data.Company.PostgreSql;


public partial class CompanyGameDb : NpgsqlUnitOfWork, ICompanyGameDb
{
    private readonly IServiceProvider _provider;

    private ICompanyGameDbFactory companyGameDbFactory => companyGameDbFactory ?? _provider.GetRequiredService<ICompanyGameDbFactory>();

    public int BackendId { get; }

    public int CompanyId { get; }

    public DataSource DataSource => DataSource.PostgreSql;

    public CompanyGameDb(IServiceProvider provider, int backendId, int companyId, NpgsqlConnection connection) 
        : base(connection)
    {
        _provider = provider;
        BackendId = backendId;
        CompanyId = companyId;
    }

    public async Task<ICompanyGameDb> CreateInstanceAsync()
    {
        var companyDb  = await companyGameDbFactory.CreateCompanyGameDbAsync(CompanyId);
        return companyDb;
    }
}


