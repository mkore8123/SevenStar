using Common.Enums;
using Infrastructure.Caching.Redis;
using Infrastructure.Data.PostgreSql;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using SevenStar.Shared.Domain.DbContext.Platform;
using SevenStar.Shared.Domain.DbContext.Platform.Entity;


namespace SevenStar.Data.Platform.PostgreSql;

public partial class PlatformDb : NpgsqlUnitOfWork, IPlatformDb
{
    private readonly IServiceProvider _provider;

    private IPlatformDbFactory platformDbFactory => platformDbFactory ?? _provider.GetRequiredService<IPlatformDbFactory>();

    public DataSource DataSource => DataSource.PostgreSql;

    public PlatformDb(IServiceProvider provider, NpgsqlConnection connection) : base(connection)
    {
        _provider = provider;
    }

    public async Task<IPlatformDb> CreateInstanceAsync()
    {
        var platformDb = await platformDbFactory.CreatePlatformDbAsync();
        return platformDb;
    }

    //public TRepository GetRepository<TRepository>() where TRepository : class, IPlatformDbContext, new()
    //{
    //    throw new NotImplementedException("GetRepository method is not implemented in PlatformDb. Use CreateInstanceAsync to create a new instance of IPlatformDb.");
    //    // return RepositoryFactoryMap.Create<TRepository>(DataSource, Connection);
    //}
}
