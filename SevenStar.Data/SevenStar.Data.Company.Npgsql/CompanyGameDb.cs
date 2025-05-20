using Npgsql;
using Common.Enums;
using Infrastructure.Data.Npgsql;
using Microsoft.Extensions.DependencyInjection;
using SevenStar.Shared.Domain.Database;

namespace SevenStar.Data.Company.Nppgsql;

public partial class CompanyGameDb : NpgsqlUnitOfWork, ICompanyGameDb
{
    private readonly IServiceProvider _provider;

    private ICompanyGameDbFactory companyGameDbFactory => companyGameDbFactory ?? _provider.GetRequiredService<ICompanyGameDbFactory>();

    public int BackendId { get; }

    public int CompanyId { get; }

    public CompanyGameDb(int companyId, IServiceProvider provider, NpgsqlConnection connection) : base(connection)
    {
        _provider = provider;
        CompanyId = companyId;
    }

    public TRepository GetRepository<TRepository>() where TRepository : class
    {
        var repository = _provider.GetRequiredKeyedService<TRepository>(DataSource.Npgsql);
        return repository;
    }

    public async Task<ICompanyGameDb> CreateInstanceAsync()
    {
        var companyDb  = await companyGameDbFactory.CreateCompanyGameDbAsync(CompanyId);
        return companyDb;
    }
}
