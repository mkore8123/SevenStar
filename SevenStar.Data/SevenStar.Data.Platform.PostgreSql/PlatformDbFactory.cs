using Infrastructure.Data.Npgsql;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;
using SevenStar.Shared.Domain.Database;


namespace SevenStar.Data.Platform.Npgsql;

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

    private NpgsqlDataSource BuildNpgsqlDataSource(string connectionString)
    {
        var builder = new NpgsqlDataSourceBuilder(connectionString);

        if (_provider.GetService<ILoggerFactory>() is ILoggerFactory loggerFactory)
        {
            builder.UseLoggerFactory(loggerFactory);
        }

        return builder.Build();
    }

    public async Task<IPlatformDb> CreatePlatformDbAsync()
    {
        var connection = await _dataSource.OpenConnectionAsync();
        return new PlatformDb(_provider, connection);
    }
}
