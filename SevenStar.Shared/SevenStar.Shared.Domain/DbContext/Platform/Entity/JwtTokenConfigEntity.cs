using System;
using System.Collections.Generic;
using System.Text;

namespace SevenStar.Shared.Domain.DbContext.Platform.Entity;

/// <summary>
/// JWT 設定 Entity，對應資料表 jwt_token_config
/// </summary>
public class JwtTokenConfigEntity
{
    /// <summary>
    /// JWT 設定主鍵，自動編號（對應 jwt_token_config.id）
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 所屬公司主鍵（對應 company.id）（對應 jwt_token_config.company_id）
    /// </summary>
    public int CompanyId { get; set; }

    /// <summary>
    /// Token 發行者（iss），如公司代碼、網址、識別字串（對應 jwt_token_config.issuer）
    /// </summary>
    public string Issuer { get; set; } = default!;

    /// <summary>
    /// Token 主要接收對象（aud），如 mobile、web、API 名稱（對應 jwt_token_config.audience）
    /// </summary>
    public string Audience { get; set; } = default!;

    /// <summary>
    /// Token 有效期間，單位為分鐘，null 表示不設定過期（對應 jwt_token_config.lifetime_minutes）
    /// </summary>
    public int? LifetimeMinutes { get; set; }

    /// <summary>
    /// 是否強制要求 Expiration Time（exp），對應 jwt_token_config.require_exp
    /// </summary>
    public bool? RequireExp { get; set; }

    /// <summary>
    /// 是否驗證 Issuer（iss），對應 jwt_token_config.validate_issuer
    /// </summary>
    public bool? ValidateIssuer { get; set; }

    /// <summary>
    /// 是否驗證 Audience（aud），對應 jwt_token_config.validate_audience
    /// </summary>
    public bool? ValidateAudience { get; set; }

    /// <summary>
    /// 是否驗證過期時間（exp），對應 jwt_token_config.validate_lifetime
    /// </summary>
    public bool? ValidateLifetime { get; set; }

    /// <summary>
    /// 容忍的時鐘誤差（單位：秒），建議預設 300 秒（對應 jwt_token_config.clock_skew_seconds）
    /// </summary>
    public int? ClockSkewSeconds { get; set; }

    /// <summary>
    /// 預設 Subject（sub），通常為用戶識別或服務帳號（對應 jwt_token_config.subject）
    /// </summary>
    public string? Subject { get; set; }

    /// <summary>
    /// Token 的 Not Before（nbf）時間（UTC）（對應 jwt_token_config.not_before）
    /// </summary>
    public DateTime? NotBefore { get; set; }

    /// <summary>
    /// Token 類型（typ），如 JWT、at+jwt（對應 jwt_token_config.token_type）
    /// </summary>
    public string? TokenType { get; set; }

    /// <summary>
    /// 預設自訂 claims（JSON 格式，key-value 配對，對應 jwt_token_config.default_claims）
    /// 儲存型態建議字串（Dictionary 請自行序列化為 JSON 字串）
    /// </summary>
    public string? DefaultClaims { get; set; }

    /// <summary>
    /// JWT header 額外欄位（JSON 格式），自訂欄位加入 header（對應 jwt_token_config.extra_header）
    /// 儲存型態建議字串（Dictionary 請自行序列化為 JSON 字串）
    /// </summary>
    public string? ExtraHeader { get; set; }

    /// <summary>
    /// JWT payload 額外欄位（JSON 格式），自訂欄位加入 payload（對應 jwt_token_config.extra_payload）
    /// 儲存型態建議字串（Dictionary 請自行序列化為 JSON 字串）
    /// </summary>
    public string? ExtraPayload { get; set; }

    /// <summary>
    /// 合法 Issuer 清單（字串陣列，可多組，對應 jwt_token_config.valid_issuers）
    /// </summary>
    public string[]? ValidIssuers { get; set; }

    /// <summary>
    /// 合法 Audience 清單（字串陣列，可多組，對應 jwt_token_config.valid_audiences）
    /// </summary>
    public string[]? ValidAudiences { get; set; }

    /// <summary>
    /// 是否啟用（現役設定，灰度/歷史追蹤/停用時可設為 false）（對應 jwt_token_config.is_active）
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// 設定版本號，支援設定異動版本化與灰度測試（對應 jwt_token_config.version_no）
    /// </summary>
    public int VersionNo { get; set; }

    /// <summary>
    /// 建立時間（UTC）（對應 jwt_token_config.created_at）
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 異動時間（UTC）（對應 jwt_token_config.updated_at）
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}
