using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Infrastructure.Data.Pg.Extensions;

/// <summary>
/// 
/// </summary>
public static class NpgsqlExtension
{
    /// <summary>
    /// 注入 PostgreSql NpgsqlDataSource，支援進階配置
    /// </summary>
    public static IServiceCollection AddNpgSqlHandler(this IServiceCollection services, string npgSqlConnectionString)
    {


        services.AddSingleton<NpgsqlDataSource>(serviceProvider =>
        {
            var dataSourceBuilder = new NpgsqlDataSourceBuilder(npgSqlConnectionString);
            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
            dataSourceBuilder.UseLoggerFactory(loggerFactory);

            return dataSourceBuilder.Build();
        });


        return services;
    }
}