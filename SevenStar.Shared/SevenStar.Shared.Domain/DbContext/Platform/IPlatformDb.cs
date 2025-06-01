using Infrastructure.Caching.Redis;
using SevenStar.Shared.Domain.DbContext.Entity.Platform;

namespace SevenStar.Shared.Domain.DbContext.Platform;

public partial interface IPlatformDb
{
    /// <summary>
    /// 取得總控遊戲資料庫連線物件
    /// </summary>
    /// <param name="backendId"></param>
    /// <returns></returns>
    Task<BackendGameDbEntity> GetBackendGameDb(int backendId);

    /// <summary>
    /// 取得公司遊戲資料庫連線物件
    /// </summary>
    /// <param name="companyId"></param>
    /// <returns></returns>
    Task<CompanyGameDbEntity> GetCompanyGameDb(int companyId);

    /// <summary>
    /// 取得公司指定用途的 redis 資料庫連線
    /// </summary>
    /// <param name="companyId"></param>
    /// <param name="redisDb"></param>
    /// <returns></returns>
    Task<CompanyRedisDbEntity> GetCompanyRedisDb(int companyId, RedisDbEnum redisDb);

    /// <summary>
    /// 取得公司的 jwt 選項設定
    /// </summary>
    /// <param name="companyId"></param>
    /// <returns></returns>
    Task<CompanyJwtOptionsEntity> GetCompanyJwtOptions(int companyId);

    /// <summary>
    /// 創建新實例的資料庫連線存取資源，避免與預設的連線物件相同，造成在平行處理使用相同連線物件造成衝突
    /// 通常用於 Parallel.ForEach 或 Task.Run 平行處理，又要使用到連線物件時使用
    /// </summary>
    /// <returns></returns>
    Task<IPlatformDb> CreateInstanceAsync();

    /// <summary>
    /// 取得指定的 Repository 物件
    /// </summary>
    /// <typeparam name="TRepository"></typeparam>
    /// <returns></returns>
    TRepository GetRepository<TRepository>() where TRepository : class, IPlatformDbContext, new();
}
