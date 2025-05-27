using Common.Enums;
using Infrastructure.Data.Npgsql;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel.Resolution;
using MySqlConnector;
using Npgsql;
using SevenStar.Shared.Domain.Database;
using SevenStar.Shared.Domain.DbContext.Repository.Company;
using SevenStar.Shared.Domain.Extensions.Repository;

namespace SevenStar.Data.Company.Nppgsql;

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

    public TRepository GetRepository<TRepository>() where TRepository : class, ICompanyGameDbContext, new()
    {
        return RepositoryFactoryMap.Create<TRepository>(DataSource, Connection);
    }

    public async Task<ICompanyGameDb> CreateInstanceAsync()
    {
        var companyDb  = await companyGameDbFactory.CreateCompanyGameDbAsync(CompanyId);
        return companyDb;
    }
}


