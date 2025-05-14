using Npgsql;
using Common.Enums;
using Infrastructure.Data.Npgsql;
using SevenStar.Shared.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace SevenStar.Data.Company.Nppgsql;

public partial class CompanyGameDb : NpgsqlUnitOfWork, ICompanyGameDb
{
    private readonly IServiceProvider _provider;   

    public CompanyGameDb(IServiceProvider provider, NpgsqlDataSource dataSource) : base(dataSource)
    {
        _provider = provider;
    }

    public TRepository GetRepository<TRepository>() where TRepository : class
    {
        var repository = _provider.GetRequiredKeyedService<TRepository>(DataSource.Npgsql);
        return repository;
    }
}
