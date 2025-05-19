using Serilog;
using StackExchange.Redis;
using Infrastructure.Caching.Redis;
using Microsoft.Extensions.DependencyInjection;

namespace SevenStar.Shared.Domain.Extensions;

public static class RedisDbExtension
{
    /// <summary>
    /// 註冊配置 Serilog 的設置項目，並將其設置為全域的日誌記錄器。
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="config">客製化的 Serilog 配置檔案，會調用 CreateLoggerConfiguration 方法，可覆寫自行調整，傳入後會啟用</param>
    /// <returns></returns>
    public static IServiceCollection AddSingleRedusDbHandler(this IServiceCollection services, string redisConnectionString)
    {
        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            return ConnectionMultiplexer.Connect(redisConnectionString);
        });

        foreach (var dbEnum in Enum.GetValues(typeof(RedisDbEnum)))
        {
            services.AddKeyedSingleton<IDatabaseAsync>(dbEnum, (sp, _) =>
            {
                var redis = sp.GetRequiredService<IConnectionMultiplexer>();
                return redis.GetDatabase((int)dbEnum);
            });
        }

        return services;
    }
}
