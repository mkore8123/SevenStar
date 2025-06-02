using Common.Api.Auth.Jwt;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Common.Api.Authentication.Jwt;

public class JwtOptions
{
    // ======================================
    // Issuer 設定（是否驗證由是否有設定 ValidIssuers 決定）
    // ======================================

    /// <summary>
    /// 合法的發行者（iss），若未設定則不驗證 Issuer
    /// </summary>
    public List<string>? ValidIssuers { get; set; }

    // ======================================
    // Audience 設定（是否驗證由是否有設定 ValidAudiences 決定）
    // ======================================

    /// <summary>
    /// 合法的接收者（aud），若未設定則不驗證 Audience
    /// </summary>
    public List<string>? ValidAudiences { get; set; }

    // ======================================
    // 簽章金鑰（是否驗證由 SigningKeys 是否設定決定）
    // ======================================

    /// <summary>
    /// 簽章金鑰清單（支援金鑰輪替）。若未設定則不驗證簽章。
    /// </summary>
    public List<JwtSigningKeyOption>? SigningKeys { get; set; }

    /// <summary>
    /// 是否要求 JWT 必須被簽章（預設 true）
    /// </summary>
    public bool RequireSignedTokens { get; set; } = true;

    /// <summary>
    /// 是否允許嘗試所有簽章金鑰來驗證（預設 true，需搭配 SigningKeys）
    /// </summary>
    public bool TryAllIssuerSigningKeys { get; set; } = true;

    // ======================================
    // Token 有效時間驗證
    // ======================================

    /// <summary>
    /// 是否強制要求有 exp（過期）欄位（預設 true）
    /// </summary>
    public bool RequireExpirationTime { get; set; } = true;

    /// <summary>
    /// 允許的時間誤差（預設 2 分鐘）
    /// </summary>
    public TimeSpan ClockSkew { get; set; } = TimeSpan.FromMinutes(2);

    /// <summary>
    /// 自訂最大 token 壽命（如設定則會觸發 Lifetime 驗證）
    /// </summary>
    public TimeSpan? MaxTokenAge { get; set; }

    // ======================================
    // 解密 JWT（JWE）用
    // ======================================

    /// <summary>
    /// JWT 為加密格式時使用的解密金鑰
    /// </summary>
    public List<string>? TokenDecryptionKeys { get; set; }

    // ======================================
    // Claim 對應欄位
    // ======================================

    /// <summary>
    /// 使用者名稱對應 ClaimType（預設 ClaimTypes.Name）
    /// </summary>
    public string NameClaimType { get; set; } = ClaimTypes.Name;

    /// <summary>
    /// 角色對應 ClaimType（預設 ClaimTypes.Role）
    /// </summary>
    public string RoleClaimType { get; set; } = ClaimTypes.Role;

    // ======================================
    // 防止 Token Replay 攻擊
    // ======================================

    /// <summary>
    /// 是否啟用重放攻擊防護（預設 false）
    /// </summary>
    public bool EnableReplayDetection { get; set; } = false;

    // ======================================
    // 其他
    // ======================================

    /// <summary>
    /// 是否保留原始 token（例如用於寫入 Cookie）
    /// </summary>
    public bool SaveSigninToken { get; set; } = false;

    /// <summary>
    /// 驗證類型（預設 jwt）
    /// </summary>
    public string AuthenticationType { get; set; } = "jwt";

    /// <summary>
    /// JWT 簽章演算法，預設為 SecurityAlgorithms.HmacSha256
    /// </summary>
    public required string SigningAlgorithm { get; set; }
}