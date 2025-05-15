using Npgsql;
using Common.Enums;
using Infrastructure.Data.Npgsql;
using SevenStar.Shared.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace SevenStar.Data.Company.Nppgsql;

public partial class CompanyGameDb : NpgsqlUnitOfWork, ICompanyGameDb
{
    private readonly IServiceProvider _provider;   

    public CompanyGameDb(IServiceProvider provider, NpgsqlConnection connection) : base(connection)
    {
        _provider = provider;
    }

    public TRepository GetRepository<TRepository>() where TRepository : class
    {
        var repository = _provider.GetRequiredKeyedService<TRepository>(DataSource.Npgsql);
        return repository;
    }

    public async Task<ICompanyGameDb> CreateNewInstance()
    {
        var npgsqlDataSource = _provider.GetRequiredService<NpgsqlDataSource>();
        var instance = new CompanyGameDb(_provider, await npgsqlDataSource.OpenConnectionAsync());

        return instance;
    }
}
