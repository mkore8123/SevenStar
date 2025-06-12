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
    Task<List<JwtSigningKeyEntity>> GetByKeyIdAsync(string keyId);

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

    /// <summary>
    /// 查詢目前所有有效（現役、尚未過期）的 JWT 簽章金鑰清單（非同步）。
    /// 僅回傳符合以下條件的金鑰：
    /// <list type="bullet">
    /// <item>is_active = true（現役啟用）</item>
    /// <item>valid_from <= 現在（生效時間已到）</item>
    /// <item>valid_to IS NULL 或 valid_to &gt; 現在（尚未過期或永久有效）</item>
    /// </list>
    /// 通常用於全域金鑰快取同步、簽章金鑰輪替、或多租戶金鑰自動切換等場景。
    /// </summary>
    /// <returns>所有有效的 JWT 簽章金鑰物件清單，若無資料則回傳空集合。</returns>
    Task<List<JwtSigningKeyEntity>?> GetAllActiveAsync();
}