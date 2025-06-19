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

    ///// <summary>
    ///// 實際使用的金鑰（對稱或非對稱）
    ///// </summary>
    //public SecurityKey SecurityKey { get; set; } = default!;

    ///// <summary>
    ///// 判斷是否為非對稱金鑰（例如 RSA、ECDSA）
    ///// </summary>
    //public bool IsAsymmetric =>
    //    SecurityKey is AsymmetricSecurityKey;

    ///// <summary>
    ///// 判斷是否為對稱金鑰（例如 AES）
    ///// </summary>
    //public bool IsSymmetric =>
    //    SecurityKey is SymmetricSecurityKey;
}

