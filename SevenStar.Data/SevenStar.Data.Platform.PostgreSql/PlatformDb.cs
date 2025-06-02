using Common.Enums;
using Infrastructure.Caching.Redis;
using Infrastructure.Data.PostgreSql;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using SevenStar.Shared.Domain.DbContext.Entity.Platform;
using SevenStar.Shared.Domain.DbContext.Platform;


namespace SevenStar.Data.Platform.PostgreSql;

public partial class PlatformDb : NpgsqlUnitOfWork, IPlatformDb
{
    private readonly IServiceProvider _provider;

    private IPlatformDbFactory platformDbFactory => platformDbFactory ?? _provider.GetRequiredService<IPlatformDbFactory>();

    public DataSource DataSource => DataSource.PostgreSql;

    public PlatformDb(IServiceProvider provider, NpgsqlConnection connection) : base(connection)
    {
        _provider = provider;
    }

    public TRepository GetRepository<TRepository>() where TRepository : class, IPlatformDbContext, new()
    {
        throw new NotImplementedException("GetRepository method is not implemented in PlatformDb. Use CreateInstanceAsync to create a new instance of IPlatformDb.");
        // return RepositoryFactoryMap.Create<TRepository>(DataSource, Connection);
    }

    public async Task<IPlatformDb> CreateInstanceAsync()
    {
        var platformDb = await platformDbFactory.CreatePlatformDbAsync();
        return platformDb;
    }

    public Task<BackendGameDbEntity> GetBackendGameDb(int backendId)
    {
        throw new NotImplementedException();
    }

    public Task<CompanyGameDbEntity> GetCompanyGameDb(int companyId)
    {
        // throw new NotImplementedException();
        var entity = new CompanyGameDbEntity
        {
            BackendId = 1, // Example value, replace with actual logic to retrieve backend ID
            CompanyId = companyId,
            ConnectionString = "Host=127.0.0.1;Port=5432;Username=postgres;Password=apeter56789;Database=postgres;SearchPath=public;", // Example connection string, replace with actual logic
            DataSource = DataSource.PostgreSql
        };

        return Task.FromResult(entity);
    }

    public Task<CompanyJwtOptionsEntity> GetCompanyJwtOptions(int companyId)
    {
        throw new NotImplementedException();
    }

    public Task<CompanyRedisDbEntity> GetCompanyRedisDb(int companyId, RedisDbEnum redisDb)
    {
        throw new NotImplementedException();
    }
}
