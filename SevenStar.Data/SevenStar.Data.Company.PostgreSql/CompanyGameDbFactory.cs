using Common.Attributes;
using Common.Enums;
using Infrastructure.Data.PostgreSql;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;
using SevenStar.Data.Company.PostgreSql;
using SevenStar.Shared.Domain.DbContext.Company;
using SevenStar.Shared.Domain.DbContext.Platform;
using System.Collections.Concurrent;

namespace SevenStar.Data.Company.PostgreSql;

[KeyedService(DataSource.PostgreSql, ServiceLifetime.Singleton)]
public class CompanyGameDbFactory : ICompanyGameDbFactory
{
    private readonly IServiceProvider _provider;
    private readonly IPlatformDbFactory _platformDbFactory;
    
    private readonly ConcurrentDictionary<int, Lazy<Task<NpgsqlDataSource>>> _companyDataSources = new();

    public CompanyGameDbFactory(IServiceProvider provider, IPlatformDbFactory platformDbFactory)
    {
        _platformDbFactory = platformDbFactory;
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
                var platformDb = await _platformDbFactory.CreatePlatformDbAsync();
                var companyGameDb = await platformDb.Company.GetCompanyGameDb(companyId);

                if (!await NpgsqlConnectionValidator.ValidateAsync(companyGameDb.ConnectionString, true))
                    throw new InvalidOperationException($"公司 ID {cid} 的連線字串無效或 Redis 連線失敗。");

                return BuildNpgsqlDataSource(companyGameDb.ConnectionString);
            }, LazyThreadSafetyMode.ExecutionAndPublication));

        try
        {
            var dataSource = await lazy.Value;
            var connection = await dataSource.OpenConnectionAsync();

            // 建立獨立的 scope provider，否則傳進去的 provider 會是 singleton
            return new CompanyGameDb(_provider.CreateScope().ServiceProvider, companyId, companyId, connection);
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

        // 建立獨立的 scope provider，否則傳進去的 provider 會是 singleton
        return new CompanyGameDb(_provider.CreateScope().ServiceProvider, baackendId, companyId, connection);
    }
}