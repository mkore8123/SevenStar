using Common.Enums;
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
    private readonly NpgsqlDataSource _platformDataSource;
    private ConcurrentDictionary<int, NpgsqlDataSource> _companyDataSource = new();
    private readonly SemaphoreSlim _semaphore = new(1, 1);


    public CompanyGameDbFactory(IServiceProvider provider, string platformConnectionString)
    {
        _provider = provider;
        _platformDataSource = BuildNpgsqlDataSource(platformConnectionString);
    }

    private NpgsqlDataSource BuildNpgsqlDataSource(string connectionString)
    {
        var builder = new NpgsqlDataSourceBuilder(connectionString);
        var loggerFactory = _provider.GetService<ILoggerFactory>();
        if (loggerFactory != null)
        {
            builder.UseLoggerFactory(loggerFactory);
        }

        return builder.Build();
    }

    private bool TryBuildNpgsqlDataSource(string connectionString, out NpgsqlDataSource? dataSource)
    {
        try
        {
            dataSource = BuildNpgsqlDataSource(connectionString);
            return true;
        }
        catch
        {
            dataSource = null;
            return false;
        }
    }

    private async Task<string> GetConnectionStringAsync(int companyId)
    {
        await using var connection = await _platformDataSource.OpenConnectionAsync();
        var repository = new CompanyRepository(connection);
        var companyModel = await repository.GetAsync(companyId) ?? throw new KeyNotFoundException("查詢不到該公司的連線資訊");

        return await Task.FromResult("Host=127.0.0.1;Port=5432;Username=postgres;Password=apeter56789;Database=postgres;SearchPath=public;");
    }

    public async Task<ICompanyGameDb> CreateCompanyGameDbAsync(int companyId)
    {
        CompanyGameDb companyDb;
        if (_companyDataSource.TryGetValue(companyId, out var existingDataSource))
        {
            companyDb = new CompanyGameDb(_provider, await existingDataSource.OpenConnectionAsync());
            return companyDb;
        }

        await _semaphore.WaitAsync();

        try
        {
            // 雙重檢查，避免其他執行緒已經建立了資料來源
            if (!_companyDataSource.TryGetValue(companyId, out existingDataSource))
            {
                var connectionString = await GetConnectionStringAsync(companyId);
                existingDataSource = BuildNpgsqlDataSource(connectionString);

                _companyDataSource.TryAdd(companyId, existingDataSource!);
            }
        }
        catch
        {
            throw;
        }
        finally
        {
            _semaphore.Release();
        }

        companyDb = new CompanyGameDb(_provider, await existingDataSource.OpenConnectionAsync());

        return companyDb;
    }
}
