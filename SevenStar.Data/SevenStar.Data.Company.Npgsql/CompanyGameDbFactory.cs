using Infrastructure.Data.Npgsql;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;
using SevenStar.Data.Company.Npgsql.Repository.Platform;
using SevenStar.Data.Company.Nppgsql;
using SevenStar.Shared.Domain;
using System.Collections.Concurrent;

namespace SevenStar.Data.Company.Npgsql;

public class CompanyGameDbFactory : ICompanyGameDbFactory
{
    private readonly IServiceProvider _provider;
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private readonly NpgsqlDataSource _platformDataSource;
    private readonly ConcurrentDictionary<int, NpgsqlDataSource> _companyDataSource = new();
    

    public CompanyGameDbFactory(IServiceProvider provider, string platformConnectionString)
    {
        _provider = provider;
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

        // 實際使用時請開啟這行
        // var companyModel = await repository.GetAsync(companyId) ?? throw new KeyNotFoundException($"公司 ID {companyId} 的連線資訊不存在");

        // 模擬用字串請勿用於正式環境
        return "Host=127.0.0.1;Port=5432;Username=postgres;Password=apeter56789;Database=postgres;SearchPath=public;";
    }

    public async Task<ICompanyGameDb> CreateCompanyGameDbAsync(int companyId)
    {
        if (companyId <= 0)
            throw new InvalidOperationException($"公司 ID {companyId} 的連線字串無效或無法連線。");

        if (_companyDataSource.TryGetValue(companyId, out var existingDataSource))
        {
            return new CompanyGameDb(companyId, _provider, await existingDataSource.OpenConnectionAsync());
        }

        await _semaphore.WaitAsync();

        try
        {
            if (!_companyDataSource.TryGetValue(companyId, out existingDataSource))
            {
                var connectionString = await GetConnectionStringAsync(companyId);
                
                if (!await NpgsqlConnectionValidator.ValidateAsync(connectionString, true))
                    throw new InvalidOperationException($"公司 ID {companyId} 的連線字串無效或無法連線。");

                existingDataSource = BuildNpgsqlDataSource(connectionString);
                _companyDataSource.TryAdd(companyId, existingDataSource);
            }
        }
        finally
        {
            _semaphore.Release();
        }

        return new CompanyGameDb(companyId, _provider, await existingDataSource.OpenConnectionAsync());
    }
}
