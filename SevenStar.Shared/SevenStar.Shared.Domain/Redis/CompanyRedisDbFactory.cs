using StackExchange.Redis;
using Infrastructure.Caching.Redis;
using System.Collections.Concurrent;
using SevenStar.Shared.Domain.DbContext.Platform;

namespace SevenStar.Shared.Domain.Redis;


public class CompanyRedisDbFactory : ICompanyRedisDbFactory
{
    private readonly int _companyId;
    private readonly IPlatformDb _platformDb;

    // 用來記錄每個用途（RedisDbEnum）對應的連線實例（延遲初始化）
    private readonly ConcurrentDictionary<RedisDbEnum, Lazy<Task<IConnectionMultiplexer>>> _connections = new();

    public CompanyRedisDbFactory(IPlatformDb platformDb, int companyId)
    {
        _platformDb = platformDb;
        _companyId = companyId;
    }

    public async Task<IDatabaseAsync> GetDatabaseAsync(RedisDbEnum purpose)
    {
        // 使用 Lazy 保證每個用途的 Redis Cluster 只初始化一次
        var lazy = _connections.GetOrAdd(purpose, key =>
            new Lazy<Task<IConnectionMultiplexer>>(async () =>
            {
                var redisDb = await _platformDb.GetCompanyRedisDb(_companyId, key);
                
                if (string.IsNullOrWhiteSpace(redisDb.RedisConnectionString))
                    throw new InvalidOperationException($"公司 ID {_companyId} 的 {key} redis連線字串無效或無法連線。");

                var config = ConfigurationOptions.Parse(redisDb.RedisConnectionString);
                return await ConnectionMultiplexer.ConnectAsync(config);
            }, LazyThreadSafetyMode.ExecutionAndPublication));

        var mux = await lazy.Value;
        return mux.GetDatabase(0); // Redis Cluster 通常僅支援 DB 0
    }
}


