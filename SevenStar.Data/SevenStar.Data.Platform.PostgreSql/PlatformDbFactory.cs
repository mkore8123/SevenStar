using Common.Attributes;
using Common.Enums;
using Infrastructure.Data.PostgreSql;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;
using SevenStar.Shared.Domain.DbContext.Platform;

namespace SevenStar.Data.Platform.PostgreSql;

[KeyedService(DataSource.PostgreSql, ServiceLifetime.Singleton)]
public class PlatformDbFactory : IPlatformDbFactory
{
    private readonly IServiceProvider _provider;
    private readonly NpgsqlDataSource _dataSource;

    public PlatformDbFactory(IServiceProvider provider, string connectionString)
    {
        if (!NpgsqlConnectionValidator.ValidateAsync(connectionString, true).GetAwaiter().GetResult())
            throw new InvalidOperationException();

        _provider = provider;
        _dataSource = BuildNpgsqlDataSource(connectionString);
    }

    /// <summary>
    /// 配置 NpgsqlDataSource 物件
    /// </summary>
    /// <param name="connectionString"></param>
    /// <returns></returns>
    private NpgsqlDataSource BuildNpgsqlDataSource(string connectionString)
    {
        var builder = new NpgsqlDataSourceBuilder(connectionString);

        if (_provider.GetService<ILoggerFactory>() is ILoggerFactory loggerFactory)
        {
            builder.UseLoggerFactory(loggerFactory);
        }

        return builder.Build();
    }

    /// <summary>
    /// 提供一個新的 PlatformDb 連線物件
    /// </summary>
    /// <returns></returns>
    public async Task<IPlatformDb> CreatePlatformDbAsync()
    {
        var connection = await _dataSource.OpenConnectionAsync();
        return new PlatformDb(_provider, connection);
    }
}
