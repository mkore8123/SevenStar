using Common.Api.Authen.Enum;
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
    /// 指定此 JWT Token 的包裝技術型態（JWS/JWE/純 JWT/嵌套等）。
    /// <para>
    /// 代表此 Token 實際的封裝安全層級與技術分類，請搭配 <see cref="JwtEnvelopeType"/> 列舉說明：
    /// <list type="table">
    /// <item><term>PlainJwt</term><description>純 JWT，無簽章無加密（極少用）</description></item>
    /// <item><term>Jws / JwsJwt</term><description>具簽章、三段結構（現今最常見）</description></item>
    /// <item><term>Jwe / JweJwt</term><description>具加密、五段結構（高機密需求場景）</description></item>
    /// <item><term>NestedJwsJwe</term><description>先簽章再加密（極高安全、少見，見 RFC 7519 §9.2）</description></item>
    /// <item><term>Custom</term><description>自訂型態（如 "at+jwt"、"id_token"、特殊協議等）</description></item>
    /// </list>
    /// <br/>
    /// <b>實務說明：</b>本欄位通常由系統依據 Token 實際內容結構（分段數、header alg/enc/typ 等）判斷與設定，
    /// 或由配置策略指定產生模式。為團隊或維運判讀 JWT 結構、選擇解碼驗簽/解密流程之依據。
    /// </para>
    /// </summary>
    public JwtEnvelopeType Type { get; set; }

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
    public string JwsKeyId { get; set; } = default!;

    /// <summary>
    /// JWS（JSON Web Signature）簽章用的金鑰物件。
    /// <para>
    /// 可為對稱金鑰（<see cref="SymmetricSecurityKey"/>，如 HMAC-SHA256）或
    /// 非對稱金鑰（<see cref="RsaSecurityKey"/>、<see cref="ECDsaSecurityKey"/>，如 RS256/ES256）。
    /// </para>
    /// <para>
    /// - 發行 JWT Token 時，用於內容簽章（根據演算法使用私鑰或密鑰）。
    /// - 驗證 JWT Token 時，用於簽章驗證（對稱金鑰或非對稱公鑰）。
    /// </para>
    /// <para>
    /// 請依據 <see cref="JwtTokenConfig.Algorithm"/> 設定金鑰型別與內容。
    /// </para>
    /// </summary>
    public SecurityKey JwsSecurityKey { get; set; } = default!;

    /// <summary>
    /// JWE 加密金鑰對應的 Key ID（kid）。
    /// 允許的值：任何合法字串（用於金鑰唯一標識，大小寫敏感）。
    /// 例如："enc-key-202406"、"jwe-key1"、"abcd1234"
    /// 
    /// 用途：
    ///   - 用於 JWE（JSON Web Encryption）場景下，指定加密用金鑰的唯一識別編號。
    ///   - 在 JWT/JWE header 產生時會對應到 "kid" 欄位，
    ///     方便接收方正確挑選解密金鑰，支援金鑰輪替與多組加密金鑰管理。
    ///   - 常用於多租戶、多環境、多版本密鑰或需要動態管理金鑰的情境。
    /// 
    /// 注意：
    ///   - "kid" 欄位僅供標識用途，本身不具保密性。
    ///   - JWE 與 JWS 的 kid 可以分開設計，分別標識簽章金鑰與加密金鑰。
    /// </summary>
    public string? JweKeyId { get; set; } = default;

    /// <summary>
    /// JWE（JSON Web Encryption）加解密用的金鑰物件。
    /// <para>
    /// 可為對稱金鑰（<see cref="SymmetricSecurityKey"/>，如 AES）
    /// 或非對稱金鑰（<see cref="RsaSecurityKey"/>，如 RSA-OAEP）。
    /// </para>
    /// <para>
    /// - 加密 JWT payload 時，用於加密資料內容。
    /// - 解密 JWE token 時，用於解密取得原始 payload。
    /// </para>
    /// <para>
    /// 請依據 <see cref="JwtTokenConfig.EncryptAlgorithm"/> 設定金鑰型別與內容。
    /// </para>
    /// </summary>
    public SecurityKey? JweSecurityKey { get; set; } = default;

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
    public string? JwsSignAlgorithm { get; set; }

    /// <summary>
    /// Token 加密演算法（JWE 金鑰管理演算法）。
    /// 允許的值：
    ///   - null（預設不加密）
    ///   - JWT/JWE 支援的演算法名稱（標準字串）
    /// 常見值如：
    ///   - "RSA-OAEP"（RSA OAEP 金鑰包裝）
    ///   - "RSA1_5"（RSA PKCS#1 v1.5 金鑰包裝）
    ///   - "A128KW"、"A256KW"（AES 金鑰包裝）
    ///   - "dir"（直接對稱金鑰使用）
    ///   - "ECDH-ES"（ECDH 密鑰協議）
    ///   - "PBES2-HS256+A128KW"（密碼推導金鑰包裝）
    ///   - 你也可以直接參考 <see cref="Microsoft.IdentityModel.Tokens.SecurityAlgorithms"/>（僅部分演算法有定義）
    /// 
    /// 用途說明：
    ///   - 指定 JWE（JSON Web Encryption）金鑰管理用演算法（對應 JWE header 的 "alg" 欄位）。
    ///   - 當需要產生加密型 JWT（JWE）時，必須指定此欄位來決定用什麼方式包裝內容加密金鑰。
    ///   - 欄位值必須與接收方支援的 JWE 金鑰管理演算法一致。
    /// 
    /// 相關標準：
    ///   - 參考 RFC 7518/JWA：https://www.rfc-editor.org/rfc/rfc7518
    /// </summary>
    public string? JweEncryptAlgorithm { get; set; }

    /// <summary>
    /// JWE 內容加密演算法（Content Encryption Algorithm）。
    /// 允許的值：
    ///   - null（未設定時不可產生 JWE）
    ///   - JWE 支援的內容加密演算法名稱（標準字串）
    /// 
    /// 常見值如：
    ///   - "A128GCM"  （AES 128 位元 GCM）
    ///   - "A192GCM"  （AES 192 位元 GCM）
    ///   - "A256GCM"  （AES 256 位元 GCM）
    ///   - "A128CBC-HS256"（AES 128 CBC + HMAC SHA-256）
    ///   - "A192CBC-HS384"（AES 192 CBC + HMAC SHA-384）
    ///   - "A256CBC-HS512"（AES 256 CBC + HMAC SHA-512）
    /// 
    /// 用途說明：
    ///   - 指定 JWE（JSON Web Encryption）實際加密 Payload 的內容加密演算法（對應 JWE header 的 "enc" 欄位）。
    ///   - 必須與金鑰管理演算法（alg）搭配使用，且雙方必須支援相同內容加密算法才能互通。
    ///   - 欄位值必須與接收方支援的內容加密演算法一致，否則無法正確解密 JWE。
    /// 
    /// 相關標準：
    ///   - 參考 RFC 7518/JWA：https://www.rfc-editor.org/rfc/rfc7518#section-5
    /// </summary>
    public string? JweContentEncryptAlgorithm { get; set; }

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
    //public EncryptingCredentials? EncryptingCredentials { get; set; }

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