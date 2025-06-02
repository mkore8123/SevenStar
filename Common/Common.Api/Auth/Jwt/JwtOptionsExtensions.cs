using Common.Api.Authentication.Jwt;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Common.Api.Auth.Jwt;

public static class JwtOptionsExtensions
{
    /// <summary>
    /// 嚴格驗證 JwtOptions 必備參數（缺少就拋出明確例外）
    /// 建議於服務初始化或產生 Token 前強制呼叫
    /// </summary>
    /// <param name="options">要驗證的 JwtOptions 實例</param>
    public static void Validate(this JwtOptions options)
    {
        // 檢查 JwtOptions 本身
        if (options == null)
            throw new ArgumentNullException(nameof(options), "JwtOptions 不可為 null！");

        // 驗證簽章金鑰必備
        if (options.SigningKeys == null || !options.SigningKeys.Any())
            throw new InvalidOperationException("JwtOptions.SigningKeys 必須至少配置一組！");

        // 驗證每一組金鑰都必須有 Key 與 KeyId
        foreach (var keyOption in options.SigningKeys)
        {
            if (string.IsNullOrWhiteSpace(keyOption.Key))
                throw new InvalidOperationException("JwtOptions.SigningKeys 中每一筆 Key 不可為空！");
            if (string.IsNullOrWhiteSpace(keyOption.KeyId))
                throw new InvalidOperationException("JwtOptions.SigningKeys 中每一筆 KeyId 不可為空！");
        }

        // 驗證簽章演算法必備
        if (string.IsNullOrWhiteSpace(options.SigningAlgorithm))
            throw new InvalidOperationException("JwtOptions.SigningAlgorithm 不可為空！");

        // 若你需要強制 Issuer/Audience 必填，也可解開下方註解
        /*
        if (options.ValidIssuers == null || !options.ValidIssuers.Any())
            throw new InvalidOperationException("JwtOptions.ValidIssuers 必須至少設定一組！");
        if (options.ValidAudiences == null || !options.ValidAudiences.Any())
            throw new InvalidOperationException("JwtOptions.ValidAudiences 必須至少設定一組！");
        */
    }

    public static TokenValidationParameters ToTokenValidationParameters(this JwtOptions options)
    {
        var parameters = new TokenValidationParameters
        {
            // =============================
            // Issuer 驗證
            // =============================

            // 是否驗證 JWT 的 `iss` 欄位。
            // 當 ValidIssuers 設定為非空時，代表需檢查 Token 的發行者是否在允許清單中。
            ValidateIssuer = options.ValidIssuers?.Any() == true,

            // 允許的發行者（iss），可多組。若未設則不驗證。
            ValidIssuers = options.ValidIssuers,

            // =============================
            // Audience 驗證
            // =============================

            // 是否驗證 JWT 的 `aud` 欄位。
            // 當 ValidAudiences 設定為非空時，才會啟用驗證。
            ValidateAudience = options.ValidAudiences?.Any() == true,

            // 允許的接收者（aud），可多組。若未設則不驗證。
            ValidAudiences = options.ValidAudiences,

            // =============================
            // 簽章驗證
            // =============================

            // 是否檢查 JWT 簽章。只要 SigningKeys 非空就必須驗證簽章。
            ValidateIssuerSigningKey = options.SigningKeys?.Any() == true,

            // 是否要求 Token 必須有簽章（JWT 規範建議為 true，避免接受未簽章的 Token）。
            RequireSignedTokens = options.RequireSignedTokens,

            // 當驗證簽章時，是否嘗試全部已設定的簽章金鑰（常用於金鑰輪替場景）。
            TryAllIssuerSigningKeys = options.TryAllIssuerSigningKeys,

            // 設定多組金鑰給 TokenValidationParameters 進行 JWT 簽章驗證。
            // 每一把金鑰都對應一個唯一的 KeyId（keyId 會自動和 JWT header 的 kid 欄位對應）。
            IssuerSigningKeys = options.SigningKeys?
                .Select(opt =>
                {
                    // 1. 以金鑰內容字串（通常是 base64 或原始 key）產生對稱加密金鑰物件。
                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(opt.Key));
                    
                    // 2. 設定這把金鑰的 KeyId（這個值必須和 JWT header 的 kid 欄位一致）
                    //    .NET 驗證流程會自動根據 kid 快速找到對應的金鑰。
                    key.KeyId = opt.KeyId;

                    // 3. 回傳 SecurityKey 實例給 TokenValidationParameters
                    return key as SecurityKey;
                }).ToList(),  // 4. 全部轉成 List 傳給 IssuerSigningKeys，.NET 驗證時會自動比對 kid

            // =============================
            // Lifetime 驗證
            // =============================

            // 是否檢查 Token 有效期間（exp, nbf），若有要求 exp 或設置最大有效期則強制啟用。
            ValidateLifetime = options.RequireExpirationTime || options.MaxTokenAge != null,

            // 是否要求 Token 必須有 exp 欄位。
            RequireExpirationTime = options.RequireExpirationTime,

            // 容許 Token 的有效時間與伺服器間的最大誤差（建議 1~5 分鐘）。
            ClockSkew = options.ClockSkew,

            // 自訂有效期驗證邏輯：可控最大 token 年齡，強化安全性。
            LifetimeValidator = (notBefore, expires, token, param) =>
            {
                var now = DateTime.UtcNow;

                // 若過期(exp)時間早於現在，直接失效。
                if (expires.HasValue && expires.Value < now)
                    return false;

                // 若 notBefore 時間比現在晚，也直接失效。
                if (notBefore.HasValue && notBefore.Value > now)
                    return false;

                // 若設定最大 token 有效期，則 issued at 時間加上最大值不可早於現在。
                if (options.MaxTokenAge.HasValue && token is JwtSecurityToken jwt)
                {
                    var issued = jwt.IssuedAt;
                    if (issued != default && issued.Add(options.MaxTokenAge.Value) < now)
                        return false;
                }

                // 其他情境皆屬於有效 token。
                return true;
            },

            // =============================
            // JWE 解密
            // =============================

            // 若 JWT 為 JWE（加密格式），提供所有支援的解密金鑰（通常也是字串轉 SymmetricSecurityKey）。
            TokenDecryptionKeys = options.TokenDecryptionKeys?
                .Select(key => new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)))
                .Cast<SecurityKey>()
                .ToList(),

            // =============================
            // Claims 對應
            // =============================

            // 用來作為 ClaimsPrincipal.Identity.Name 的 claim 欄位名稱，預設為 "name"。
            NameClaimType = options.NameClaimType,

            // 用來作為角色判斷依據的 claim 欄位名稱，預設為 "role"。
            RoleClaimType = options.RoleClaimType,

            // =============================
            // Token Replay 攻擊防護
            // =============================

            // 是否啟用 Token 重放攻擊防護（需搭配 ITokenReplayCache 實作）。
            ValidateTokenReplay = options.EnableReplayDetection,

            // =============================
            // 其他
            // =============================

            // 是否保留原始 Token（例如在驗證通過後寫入 Cookie）。
            SaveSigninToken = options.SaveSigninToken,

            // 自訂驗證類型名稱（例如 "jwt"），便於多組驗證機制管理。
            AuthenticationType = options.AuthenticationType
        };

        return parameters;
    }
}