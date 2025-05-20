using Infrastructure.Data.Npgsql;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;
using SevenStar.Data.Company.Npgsql.Repository.Platform;
using SevenStar.Data.Company.Nppgsql;
using SevenStar.Shared.Domain.Database;
using System.Collections.Concurrent;

namespace SevenStar.Data.Company.Npgsql;

public class CompanyGameDbFactory : ICompanyGameDbFactory
{
    private readonly IServiceProvider _provider;
    private readonly NpgsqlDataSource _platformDataSource;
    private readonly ConcurrentDictionary<int, Lazy<Task<NpgsqlDataSource>>> _companyDataSources = new();

    public CompanyGameDbFactory(IServiceProvider provider, string platformConnectionString)
    {
        _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        _platformDataSource = BuildNpgsqlDataSource(platformConnectionString);
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

    private async Task<string> GetConnectionStringAsync(int companyId)
    {
        await using var connection = await _platformDataSource.OpenConnectionAsync();
        var repository = new CompanyRepository(connection);

        // TODO: 改為實際從資料庫取連線字串
        // var companyModel = await repository.GetAsync(companyId)
        //     ?? throw new KeyNotFoundException($"公司 ID {companyId} 的連線資訊不存在");

        return "Host=127.0.0.1;Port=5432;Username=postgres;Password=apeter56789;Database=postgres;SearchPath=public;";
    }

    public async Task<ICompanyGameDb> CreateCompanyGameDbAsync(int companyId)
    {
        if (companyId <= 0)
            throw new ArgumentOutOfRangeException(nameof(companyId), "公司 ID 無效。");

        var lazy = _companyDataSources.GetOrAdd(companyId, cid =>
            new Lazy<Task<NpgsqlDataSource>>(async () =>
            {
                var connStr = await GetConnectionStringAsync(cid);

                if (!await NpgsqlConnectionValidator.ValidateAsync(connStr, true))
                    throw new InvalidOperationException($"公司 ID {cid} 的連線字串無效或 Redis 連線失敗。");

                return BuildNpgsqlDataSource(connStr);
            }));

        try
        {
            var dataSource = await lazy.Value;
            var connection = await dataSource.OpenConnectionAsync();
            return new CompanyGameDb(companyId, _provider, connection);
        }
        catch
        {
            _companyDataSources.TryRemove(companyId, out _); // 若初始化失敗，清除快取避免卡死
            throw;
        }
    }
}