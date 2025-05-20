using Infrastructure.Caching.Redis;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace SevenStar.Shared.Domain.RedisCache;

public interface ICompanyRedisDbFactory
{
    /// <summary>
    /// 取得指定用途的 Redis 資料庫連線
    /// </summary>
    /// <param name="purpose">用途</param>
    /// <returns></returns>
    Task<IDatabaseAsync> GetDatabaseAsync(RedisDbEnum purpose);
}