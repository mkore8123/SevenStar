using Npgsql;
using Infrastructure.Data.Npgsql;
using Infrastructure.Caching.Redis;
using SevenStar.Shared.Domain.Database;
using SevenStar.Shared.Domain.Entity.Platform;

namespace SevenStar.Data.Platform.Npgsql;

public class PlatformDb : NpgsqlUnitOfWork, IPlatformDb
{
    private readonly IServiceProvider _provider;

    public PlatformDb(IServiceProvider provider, NpgsqlConnection connection) : base(connection)
    {
        _provider = provider;
    }

    public Task<CompanyRedisDbEntity> GetCompanyRedisDb(int companyId, RedisDbEnum redisDb)
    {
        throw new NotImplementedException();
    }

    public Task<List<CompanyRedisDbEntity>> GetCompanyRedisDb(int companyId)
    {
        throw new NotImplementedException();
    }

    public TRepository GetRepository<TRepository>() where TRepository : IPlatformDb, new()
    {
        throw new NotImplementedException();
    }
}
