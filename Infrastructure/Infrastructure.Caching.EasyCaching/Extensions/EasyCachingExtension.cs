using EasyCaching.Core.Configurations;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Api.Extensions;

/// <summary>
/// 配置混合快取服務
/// </summary>
public static class EasyCachingExtension
{
    /// <summary>
    /// 註冊例外處理所需服務
    /// </summary>
    public static IServiceCollection AddHybridCacheHandler(this IServiceCollection services, string redisConnectionString)
    {       
        services.AddEasyCaching(options =>
        {
            options.WithJson("default"); // ✅ 重點：補上 Json 序列化器

            options.UseInMemory("inmem");

            options.UseRedis(config =>
            {
                config.DBConfig.Endpoints.Add(new ServerEndPoint("localhost", 6379));
                config.DBConfig.Database = 0;
                config.SerializerName = "default"; // ✅ 明確指定序列化器
            }, "redis1");

            options.UseHybrid(config =>
            {
                config.TopicName = "easycaching.hybrid";
                config.LocalCacheProviderName = "inmem";
                config.DistributedCacheProviderName = "redis1";
            }, "hybrid");

            options.WithRedisBus(bus =>
            {
                bus.Configuration = "localhost:6379";
                bus.SerializerName = "default"; // ✅ 明確指定序列化器
            });
        });

        return services;
    }
}