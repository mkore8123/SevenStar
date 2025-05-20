using Infrastructure.Caching.Redis;
using SevenStar.Shared.Domain.Entity.Platform;
using System;
using System.Collections.Generic;
using System.Text;

namespace SevenStar.Shared.Domain.Database;

public interface IPlatformDb
{
    /// <summary>
    /// 取得公司所有用途的 redis 資料庫連線
    /// </summary>
    /// <param name="companyId"></param>
    /// <returns></returns>
    Task<List<CompanyRedisDbEntity>> GetCompanyRedisDb(int companyId);

    /// <summary>
    /// 取得公司指定用途的 redis 資料庫連線
    /// </summary>
    /// <param name="companyId"></param>
    /// <param name="redisDb"></param>
    /// <returns></returns>
    Task<CompanyRedisDbEntity> GetCompanyRedisDb(int companyId, RedisDbEnum redisDb);
}
