using Common.Attributes;
using Common.Enums;
using Infrastructure.Data.Npgsql;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;
using SevenStar.Data.Company.Nppgsql;
using SevenStar.Shared.Domain.Database;
using System.Collections.Concurrent;
using System.ComponentModel.Design;

namespace SevenStar.Data.Company.Npgsql;

[KeyedService(DataSource.PostgreSql, ServiceLifetime.Singleton)]
public class CompanyGameDbFactory : ICompanyGameDbFactory
{
    private readonly IPlatformDb _platformDb;
    private readonly IServiceProvider _provider;
    private readonly ConcurrentDictionary<int, Lazy<Task<NpgsqlDataSource>>> _companyDataSources = new();

    public CompanyGameDbFactory(IServiceProvider provider, IPlatformDb platformDb)
    {
        _platformDb = platformDb;
        _provider = provider ?? throw new ArgumentNullException(nameof(provider));
    }

    private NpgsqlDataSource BuildNpgsqlDataSource(string connectionString)
    {
        var builder = new NpgsqlDataSourceBuilder(connectionString);

        if (_provider.GetService<ILoggerFactory>() is ILoggerFactory loggerFactory)
        {
            builder.UseLoggerFactory(loggerFactory);
        }

        return builder.Build();
    }

    public async Task<ICompanyGameDb> CreateCompanyGameDbAsync(int companyId)
    {
        if (companyId <= 0)
            throw new ArgumentOutOfRangeException(nameof(companyId), "公司 ID 無效。");

        var lazy = _companyDataSources.GetOrAdd(companyId, cid =>
            new Lazy<Task<NpgsqlDataSource>>(async () =>
            {
                var companyDb = await _platformDb.GetCompanyGameDb(companyId);

                if (!await NpgsqlConnectionValidator.ValidateAsync(companyDb.ConnectionString, true))
                    throw new InvalidOperationException($"公司 ID {cid} 的連線字串無效或 Redis 連線失敗。");

                return BuildNpgsqlDataSource(companyDb.ConnectionString);
            }));

        try
        {
            var dataSource = await lazy.Value;
            var connection = await dataSource.OpenConnectionAsync();
            return new CompanyGameDb(_provider, companyId, companyId, connection);
        }
        catch
        {
            _companyDataSources.TryRemove(companyId, out _); // 若初始化失敗，清除快取避免卡死
            throw;
        }
    }

    public async Task<ICompanyGameDb> CreateCompanyGameDbAsync(int baackendId, int companyId, string connectionString)
    {
        var dataSource = BuildNpgsqlDataSource(connectionString);
        var connection = await dataSource.OpenConnectionAsync();

        return new CompanyGameDb(_provider, baackendId, companyId, connection);
    }
}