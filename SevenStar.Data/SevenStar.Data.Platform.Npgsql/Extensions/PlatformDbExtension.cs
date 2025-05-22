
using Infrastructure.Data.Npgsql;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;
using SevenStar.Shared.Domain.Database;
using SevenStar.Shared.Domain.Entity.Company;

namespace SevenStar.Data.Platform.Npgsql.Extensions;

/// <summary>
/// 
/// </summary>
public static class PlatformDbExtension
{
    /// <summary>
    /// 注入 PostgreSql NpgsqlDataSource，支援進階配置
    /// </summary>
    public static IServiceCollection AddPlatformDbHandler(this IServiceCollection services, IServiceProvider provider, string platformConnectionString)
    {
        services.AddSingleton<IPlatformDb>(serviceProvider =>
        {
            var factory = serviceProvider.GetRequiredService<IPlatformDbFactory>();
            var db = factory.CreatePlatformDbAsync().GetAwaiter().GetResult();

            return db;
        });

        return services;
    }
}