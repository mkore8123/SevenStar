using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Api.Authen.Jwt.Model;

/// <summary>
/// JWT 簽章金鑰設定（JWS）
/// </summary>
public class JwtSigningKey
{
    /// <summary>
    /// 簽章演算法，如 RS256、HS256、ES256
    /// </summary>
    public string Algorithm { get; set; } = default!;

    /// <summary>
    /// 金鑰唯一識別 ID（對應 Header 的 kid）
    /// </summary>
    public string KeyId { get; set; } = default!;

    /// <summary>
    /// Base64 或 PEM 格式的 Public Key（適用於 RSA / EC / 對稱加密）
    /// </summary>
    public string? PublicKey { get; set; }

    /// <summary>
    /// Base64 或 PEM 格式的 Private Key（如為對稱式則存一份即可）
    /// </summary>
    public string? PrivateKey { get; set; }
}
