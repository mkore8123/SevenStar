using Npgsql;
using Common.Enums;
using Infrastructure.Data.Npgsql;
using SevenStar.Shared.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace SevenStar.Data.Company.Nppgsql;

public partial class CompanyGameDb : NpgsqlUnitOfWork, ICompanyGameDb
{
    private readonly IServiceProvider _provider;

    public int CompanyId { get; }

    public CompanyGameDb(IServiceProvider provider, int companyId, NpgsqlConnection connection) : base(connection)
    {
        _provider = provider;
        CompanyId = companyId;
    }

    public TRepository GetRepository<TRepository>() where TRepository : class
    {
        var repository = _provider.GetRequiredKeyedService<TRepository>(DataSource.Npgsql);
        return repository;
    }

    public async Task<ICompanyGameDb> CreateNewInstanceAsync()
    {
        var companyFactory = _provider.GetRequiredService<ICompanyGameDbFactory>();
        var companyDb  = await companyFactory.CreateCompanyGameDbAsync(CompanyId);

        return companyDb;
    }
}
