using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Caching.Redis.Interface;

/// <summary>
///  Redis Hash 轉換為物件的介面
/// </summary>
public interface IRedisHashInitializable : IRedisKey
{
    /// <summary>
    /// 將 Redis Hash 物件欄位值為自訂物件初始化
    /// </summary>
    /// <param name="entries"></param>
    void LoadFromHash(HashEntry[] entries);

    /// <summary>
    /// 將自訂物件轉換為 Redis Hash 物件
    /// </summary>
    /// <returns></returns>
    HashEntry[] ConvertToHash();
}
