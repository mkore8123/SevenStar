using SevenStar.Shared.Domain.DbContext.Platform.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace SevenStar.Shared.Domain.DbContext.Platform.Repository;

/// <summary>
/// JWT 加密金鑰（JWE）資料存取 Repository 介面，提供加密金鑰的非同步 CRUD 與查詢功能。
/// </summary>
public interface IJwtEncryptingKeyRepository
{
    /// <summary>
    /// 新增一筆 JWT 加密金鑰（非同步）。
    /// </summary>
    /// <param name="key">要新增的 JWT 加密金鑰物件。</param>
    /// <returns>資料庫自動產生的主鍵 id。</returns>
    Task<int> CreateAsync(JwtEncryptingKeyEntity key);

    /// <summary>
    /// 依主鍵查詢 JWT 加密金鑰（非同步）。
    /// </summary>
    /// <param name="id">JWT 加密金鑰主鍵 id。</param>
    /// <returns>金鑰物件，查無則回傳 null。</returns>
    Task<JwtEncryptingKeyEntity?> GetByIdAsync(int id);

    /// <summary>
    /// 依 config_id 取得所有金鑰（非同步）。
    /// </summary>
    /// <param name="configId">對應 JWT 設定主鍵。</param>
    /// <returns>該 config_id 下所有金鑰物件清單。</returns>
    Task<List<JwtEncryptingKeyEntity>> GetByConfigIdAsync(int configId);

    /// <summary>
    /// 依 key_id 查詢金鑰（非同步）。
    /// </summary>
    /// <param name="keyId">JWT Header 內的 kid（KeyId）。</param>
    /// <returns>所有對應 kid 的金鑰物件清單。</returns>
    Task<List<JwtEncryptingKeyEntity>> GetByKeyIdAsync(string keyId);

    /// <summary>
    /// 更新一筆 JWT 加密金鑰（非同步，依 id 覆寫）。
    /// </summary>
    /// <param name="key">包含主鍵 id 的金鑰物件。</param>
    Task UpdateAsync(JwtEncryptingKeyEntity key);

    /// <summary>
    /// 刪除指定主鍵的 JWT 加密金鑰（非同步）。
    /// </summary>
    /// <param name="id">金鑰主鍵 id。</param>
    Task DeleteAsync(int id);

    /// <summary>
    /// 查詢所有目前有效（現役、未過期）的 JWT 加密金鑰（非同步）。
    /// 只回傳 is_active = true，valid_from 已生效，且 valid_to 尚未過期或為 null 的金鑰。
    /// 適合全域快取預熱、金鑰同步等場景。
    /// </summary>
    /// <returns>所有有效的加密金鑰清單，若無則回傳空集合。</returns>
    Task<List<JwtEncryptingKeyEntity>?> GetAllActiveAsync();
}
