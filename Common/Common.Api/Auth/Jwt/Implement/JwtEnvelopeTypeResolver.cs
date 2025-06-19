using Common.Api.Authen.Jwt.Enum;
using Common.Api.Authen.Jwt.Exception;
using Common.Api.Authen.Jwt.Interface;
using System.Text.Json;

namespace Common.Api.Authen.Jwt.Implement;

/// <summary>
/// 預設 JWT envelope 型態解析器
/// 支援 JWS（簽章型）、JWE（加密型）、Nested JwsJwe（先簽章再加密）
/// </summary>
public class JwtEnvelopeTypeResolver : IJwtEnvelopeTypeResolver
{
    public JwtEnvelopeType Resolve(string jwtToken)
    {
        if (string.IsNullOrWhiteSpace(jwtToken))
            throw new ArgumentNullException(nameof(jwtToken), "JWT Token 不可為空");

        var parts = jwtToken.Split('.');
        if (parts.Length == 3)
        {
            // 3 段為 JWS
            var headerJson = Base64UrlDecode(parts[0]);
            using var headerDoc = JsonDocument.Parse(headerJson);
            var root = headerDoc.RootElement;
            // JWS header 若有 enc（極少見，防呆）
            if (root.TryGetProperty("enc", out _))
                return JwtEnvelopeType.Jwe;
            return JwtEnvelopeType.Jws;
        }
        else if (parts.Length == 5)
        {
            // 5 段為 JWE 或 Nested
            var headerJson = Base64UrlDecode(parts[0]);
            using var headerDoc = JsonDocument.Parse(headerJson);
            var root = headerDoc.RootElement;

            // 有 cty: "JWT" (不區分大小寫) 判為 NestedJwsJwe，否則 JWE
            var isNested = root.TryGetProperty("cty", out var ctyProp) &&
                           ctyProp.GetString()?.Equals("JWT", StringComparison.OrdinalIgnoreCase) == true;
            if (isNested)
                return JwtEnvelopeType.NestedJwsJwe;

            return JwtEnvelopeType.Jwe;
        }
        else
        {
            throw new InvalidJwtException("無法辨識的 JWT 格式（非 3 或 5 段）");
        }
    }

    /// <summary>
    /// RFC 7515/7516 標準 Base64Url 解碼
    /// </summary>
    private static string Base64UrlDecode(string base64Url)
    {
        string padded = base64Url.Replace('-', '+').Replace('_', '/');
        switch (padded.Length % 4)
        {
            case 2: padded += "=="; break;
            case 3: padded += "="; break;
        }
        var bytes = Convert.FromBase64String(padded);
        return System.Text.Encoding.UTF8.GetString(bytes);
    }
}

