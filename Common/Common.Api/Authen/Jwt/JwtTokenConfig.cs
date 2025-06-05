using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Api.Auth.Jwt;

/// <summary>
/// JWT Token 設定參數
/// </summary>
public class JwtTokenConfig
{
    /// <summary>
    /// Token 發行者（iss）。
    /// 允許的值：任何合法字串（通常為 URI、URL、公司/服務名稱等）。
    /// 例如："https://auth.example.com"、"mycompany"、"partnerX"
    /// 不可為 null 或空字串。
    /// </summary>
    public string Issuer { get; set; } = default!;

    /// <summary>
    /// Token 主要接收對象（aud）。
    /// 允許的值：任何合法字串（通常代表應用名稱、裝置類型、服務識別）。
    /// 例如："web"、"mobile"、"api-client"、"https://app.example.com"
    /// 不可為 null 或空字串。
    /// </summary>
    public string Audience { get; set; } = default!;

    /// <summary>
    /// JWT 金鑰對應的 Key ID（kid）。
    /// 允許的值：任何合法字串（用於金鑰唯一標識，大小寫敏感）。
    /// 例如："key1"、"rsa-key-202406"、"abcd1234"
    /// 不可為 null 或空字串。
    /// </summary>
    public string KeyId { get; set; } = default!;

    /// <summary>
    /// Token 有效期間（存活時間）。
    /// 允許的值：null（不設定），或正數的 TimeSpan。
    /// 常見值如：TimeSpan.FromMinutes(60)、TimeSpan.FromDays(1)
    /// null 表示未指定過期時間（極少見，建議至少設定一個過期時間）。
    /// </summary>
    public TimeSpan? Lifetime { get; set; }

    /// <summary>
    /// Token 簽章演算法。
    /// 允許的值：null（預設 HmacSha256），或 JWT 支援的演算法名稱。
    /// 常見如："HS256"、"RS256"、"ES256"、SecurityAlgorithms.HmacSha256 等。
    /// 請參考 Microsoft.IdentityModel.Tokens.SecurityAlgorithms 靜態欄位。
    /// </summary>
    public string? Algorithm { get; set; }

    /// <summary>
    /// 是否強制要求 Expiration Time（exp）。
    /// 允許的值：null（預設 true），或 true/false。
    /// true = 必須要有 exp，false = 可不加 exp。
    /// </summary>
    public bool? RequireExpirationTime { get; set; }

    /// <summary>
    /// 是否驗證 Issuer（iss）。
    /// 允許的值：null（預設 true），或 true/false。
    /// true = 必須比對 issuer，false = 不驗證。
    /// </summary>
    public bool? ValidateIssuer { get; set; }

    /// <summary>
    /// 是否驗證 Audience（aud）。
    /// 允許的值：null（預設 true），或 true/false。
    /// true = 必須比對 audience，false = 不驗證。
    /// </summary>
    public bool? ValidateAudience { get; set; }

    /// <summary>
    /// 是否驗證過期時間（exp）。
    /// 允許的值：null（預設 true），或 true/false。
    /// true = 必須檢查 exp，false = 不檢查過期。
    /// </summary>
    public bool? ValidateLifetime { get; set; }

    /// <summary>
    /// 容忍的時鐘誤差範圍（Clock Skew）。
    /// 允許的值：null（預設 5 分鐘），或 TimeSpan > 0。
    /// 例如：TimeSpan.FromMinutes(5)
    /// </summary>
    public TimeSpan? ClockSkew { get; set; }

    /// <summary>
    /// 合法 Issuer 清單（多組）。
    /// 允許的值：null（不設定），或字串清單（不能含 null 或空字串）。
    /// 例如：["https://a.com", "https://b.com"]。空集合代表不限制。
    /// </summary>
    public List<string>? ValidIssuers { get; set; }

    /// <summary>
    /// 合法 Audience 清單（多組）。
    /// 允許的值：null（不設定），或字串清單（不能含 null 或空字串）。
    /// 例如：["web", "mobile", "api-client"]。空集合代表不限制。
    /// </summary>
    public List<string>? ValidAudiences { get; set; }

    /// <summary>
    /// Token 預設 Subject（sub）。
    /// 允許的值：null（不設定），或任意合法字串（通常是用戶識別）。
    /// 例如："user123"、"service-account"
    /// </summary>
    public string? Subject { get; set; }

    /// <summary>
    /// Token 的 Not Before 時間（nbf）。
    /// 允許的值：null（不設定），或合法 UTC 時間。
    /// 例如：DateTime.UtcNow，或指定一個未來時間。
    /// </summary>
    public DateTime? NotBefore { get; set; }

    /// <summary>
    /// 預設自訂 claims。
    /// 允許的值：null（不設定），或 key/value 字典（不能含 null）。
    /// 例如：{ "role": "admin", "tenant": "a" }
    /// </summary>
    public Dictionary<string, string>? DefaultClaims { get; set; }

    /// <summary>
    /// Token 類型（typ）。
    /// 允許的值：null（不設定 typ），或常見字串如 "JWT"、"at+jwt"。
    /// </summary>
    public string? TokenType { get; set; }

    /// <summary>
    /// Token 唯一識別（jti）產生函式。
    /// 允許的值：null（不產生 jti），或 delegate 傳回唯一字串。
    /// 例如：() => Guid.NewGuid().ToString()
    /// </summary>
    public Func<string>? JtiGenerator { get; set; }

    /// <summary>
    /// JWT 加密憑證（EncryptingCredentials）。
    /// 允許的值：
    ///   - null（預設，不做加密，僅簽章）
    ///   - EncryptingCredentials 物件（用於產生 JWE，即加密型 JWT）
    /// 用途：
    ///   - 如果你需要「機密性」（內容只有特定收件者能解密）而非只有簽章（不可否認性），
    ///     可以提供 EncryptingCredentials，會將 payload 以對稱或非對稱演算法加密。
    ///   - 常用於高度敏感資訊、跨企業傳遞時不希望資料被第三方看到時。
    ///   - JWE（JSON Web Encryption）支援，但一般 API 驗證主要用 JWS（簽章即可）。
    /// 參考：
    ///   - https://datatracker.ietf.org/doc/html/rfc7516
    /// </summary>
    public EncryptingCredentials? EncryptingCredentials { get; set; }

    /// <summary>
    /// JWT Header 額外欄位（ExtraHeader）。
    /// 允許的值：
    ///   - null（預設不加）
    ///   - Dictionary&lt;string, object&gt;，自訂要加進 JWT Header 的其他欄位
    /// 用途：
    ///   - 某些特殊協議、廠商要求，或想加入額外描述（如自訂欄位、traceId、協議版本等）。
    ///   - 會原封不動寫進 JWT Header 部分（跟 typ、alg、kid 同層）。
    /// 典型值：
    ///   - { "traceId": "123456" }
    ///   - { "ver": "2", "customHeader": "value" }
    /// 注意：
    ///   - Header 欄位內容不具備保密性，但會參與簽章驗證。
    /// </summary>
    public Dictionary<string, object>? ExtraHeader { get; set; }

    /// <summary>
    /// JWT Payload 額外欄位（ExtraPayload）。
    /// 允許的值：
    ///   - null（預設不加）
    ///   - Dictionary&lt;string, object&gt;，自訂要加進 JWT Payload 的其他欄位
    /// 用途：
    ///   - 用於額外附加非標準（iss, aud, sub, exp, nbf 等以外）的自訂 claims。
    ///   - 可動態加任何資料，例如 roles、tenant、公司 id、session、extra info。
    /// 典型值：
    ///   - { "roles": ["admin", "user"], "department": "sales" }
    ///   - { "scopes": "api.read api.write" }
    /// 注意：
    ///   - payload 欄位內容不具備保密性（除非你同時用 EncryptingCredentials 做加密），
    ///     但會參與簽章，內容不可竄改。
    /// </summary>
    public Dictionary<string, object>? ExtraPayload { get; set; }
}