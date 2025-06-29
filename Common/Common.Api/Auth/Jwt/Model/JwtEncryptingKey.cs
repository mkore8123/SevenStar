using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Api.Authen.Jwt.Model;

/// <summary>
/// JWT 加密金鑰設定（JWE）
/// </summary>
public class JwtEncryptingKey
{
    /// <summary>
    /// 金鑰管理演算法（alg），如 RSA-OAEP、A256KW、dir
    /// </summary>
    public string Algorithm { get; set; } = default!;

    /// <summary>
    /// 內容加密演算法（enc），如 A256GCM、A256CBC-HS512
    /// </summary>
    public string ContentAlgorithm { get; set; } = default!;

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

