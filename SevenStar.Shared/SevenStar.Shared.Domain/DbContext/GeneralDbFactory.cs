using Microsoft.Extensions.DependencyInjection;
using SevenStar.Shared.Domain.Database;
using SevenStar.Shared.Domain.DbContext.Entity.Platform;
using System.Collections.Concurrent;

namespace SevenStar.Shared.Domain.DbContext;

/// <summary>
/// 路由器：根據公司 ID 找出所需的資料庫類型，並呼叫對應的 Factory 來產出 ICompanyGameDb
/// </summary>
public partial class GeneralDbFactory : IGeneralDbFactory
{
    /// <summary>
    /// 平台庫
    /// </summary>
    private readonly IPlatformDb _platformDb;
    private readonly ISingletonCacheService _cache;
    private readonly IServiceProvider _provider;
    
    // 快取：companyId => 資料庫類型 (DataSource)
    private readonly ConcurrentDictionary<int, Lazy<Task<CompanyGameDbEntity>>> _companyDbSourceCache = new();

    public GeneralDbFactory(IServiceProvider provider, IPlatformDb platformDb,
        ISingletonCacheService cache)
    {
        _provider = provider;
        _platformDb = platformDb;
        _cache = cache;
    }

    public async Task<ICompanyGameDb> CreateCompanyGameDbAsync(int companyId)
    {
        if (companyId <= 0)
            throw new ArgumentOutOfRangeException(nameof(companyId));

        var entity = await _cache.GetOrAddCompnayDbAsync(companyId, () => _platformDb.GetCompanyGameDb(companyId));
        var factory = _provider.GetRequiredKeyedService<ICompanyGameDbFactory>(entity.DataSource);

        return await factory.CreateCompanyGameDbAsync(
            entity.BackendId,
            entity.CompanyId,
            entity.ConnectionString);
    }
}
