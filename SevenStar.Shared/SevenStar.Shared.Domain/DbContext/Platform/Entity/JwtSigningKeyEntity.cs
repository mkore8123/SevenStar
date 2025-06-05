using System;
using System.Collections.Generic;
using System.Text;

namespace SevenStar.Shared.Domain.DbContext.Platform.Entity;

/// <summary>
/// JWT 金鑰管理 Entity，對應資料表 jwt_signing_key
/// </summary>
public class JwtSigningKeyEntity
{
    /// <summary>
    /// JWT 金鑰主鍵，自動編號（對應 jwt_signing_key.id）
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 所屬 JWT 設定主鍵（對應 jwt_token_config.id）（對應 jwt_signing_key.config_id）
    /// </summary>
    public int ConfigId { get; set; }

    /// <summary>
    /// Key ID（kid），JWT Header 內標示的金鑰編號（對應 jwt_signing_key.key_id）
    /// </summary>
    public string KeyId { get; set; } = default!;

    /// <summary>
    /// JWT 金鑰簽章演算法，如 HS256、RS256、ES256 等（對應 jwt_signing_key.algorithm）
    /// </summary>
    public string Algorithm { get; set; } = default!;

    /// <summary>
    /// 公鑰內容（Text），僅用於非對稱金鑰演算法（RSA/ECDSA），對稱時可留空（對應 jwt_signing_key.public_key）
    /// </summary>
    public string? PublicKey { get; set; }

    /// <summary>
    /// 私鑰內容（Text），僅發 token 端需存放，可加密或用 HSM 管理（對應 jwt_signing_key.private_key）
    /// </summary>
    public string? PrivateKey { get; set; }

    /// <summary>
    /// 金鑰啟用時間，僅限於此時間之後簽發/驗證有效（對應 jwt_signing_key.valid_from）
    /// </summary>
    public DateTime ValidFrom { get; set; }

    /// <summary>
    /// 金鑰失效時間，到期後不再簽發/驗證（null 表示永久，對應 jwt_signing_key.valid_to）
    /// </summary>
    public DateTime? ValidTo { get; set; }

    /// <summary>
    /// 金鑰是否啟用（現役）（對應 jwt_signing_key.is_active）
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// 金鑰建立時間（UTC）（對應 jwt_signing_key.created_at）
    /// </summary>
    public DateTime CreatedAt { get; set; }
}