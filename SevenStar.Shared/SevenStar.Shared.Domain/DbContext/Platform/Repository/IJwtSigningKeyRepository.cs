using SevenStar.Shared.Domain.DbContext.Platform.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace SevenStar.Shared.Domain.DbContext.Platform.Repository;

/// <summary>
/// JWT 金鑰管理 Repository 介面，提供金鑰 CRUD 非同步操作
/// </summary>
public interface IJwtSigningKeyRepository
{
    /// <summary>
    /// 新增一把金鑰（非同步）
    /// </summary>
    /// <param name="key">JWT 金鑰物件</param>
    /// <returns>資料庫產生的金鑰主鍵 id</returns>
    Task<int> CreateAsync(JwtSigningKeyEntity key);

    /// <summary>
    /// 依主鍵查詢金鑰（非同步）
    /// </summary>
    /// <param name="id">金鑰主鍵 id</param>
    /// <returns>JWT 金鑰物件，查無則回傳 null</returns>
    Task<JwtSigningKeyEntity?> GetByIdAsync(int id);

    /// <summary>
    /// 依 JWT 設定（config_id）查詢所有金鑰（非同步）
    /// </summary>
    /// <param name="configId">JWT 設定主鍵 id</param>
    /// <returns>JWT 金鑰物件清單</returns>
    Task<List<JwtSigningKeyEntity>> GetByConfigIdAsync(int configId);

    /// <summary>
    /// 依 key_id 查詢所有金鑰（同一 kid 可能有多組設定/多把金鑰）（非同步）
    /// </summary>
    /// <param name="keyId">JWT 金鑰編號（kid）</param>
    /// <returns>JWT 金鑰物件清單</returns>
    Task<IEnumerable<JwtSigningKeyEntity>> GetByKeyIdAsync(string keyId);

    /// <summary>
    /// 更新金鑰資料（依 id 覆寫）（非同步）
    /// </summary>
    /// <param name="key">JWT 金鑰物件（需帶主鍵 id）</param>
    Task UpdateAsync(JwtSigningKeyEntity key);

    /// <summary>
    /// 刪除金鑰（依主鍵 id）（非同步）
    /// </summary>
    /// <param name="id">金鑰主鍵 id</param>
    Task DeleteAsync(int id);
}