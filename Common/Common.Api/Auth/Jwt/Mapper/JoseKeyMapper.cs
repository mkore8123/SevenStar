using Common.Api.Authen.Jwt.Model;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Common.Api.Auth.Jwt.Mapper;

public static class JoseKeyMapper
{
    /// <summary>
    /// 將 JwtSigningKey 轉換為 Jose.JWT 套件支援的 key 實例（byte[]、RSA、ECDsa）。
    /// </summary>
    public static object ToJoseKey(JwtSigningKey key)
    {
        if (key.Algorithm.StartsWith("HS", StringComparison.OrdinalIgnoreCase))
        {
            // HMAC（對稱式）：使用 PrivateKey 當作 byte[]
            if (string.IsNullOrWhiteSpace(key.PrivateKey))
                throw new ArgumentException("HMAC 演算法需提供 PrivateKey");

            return Encoding.UTF8.GetBytes(key.PrivateKey);
        }

        if (key.Algorithm.StartsWith("RS", StringComparison.OrdinalIgnoreCase))
        {
            // RSA（非對稱式）
            if (string.IsNullOrWhiteSpace(key.PrivateKey))
                throw new ArgumentException("RSA 演算法需提供 PrivateKey");

            var rsa = RSA.Create();
            rsa.ImportFromPem(key.PrivateKey.ToCharArray());
            return rsa;
        }

        if (key.Algorithm.StartsWith("ES", StringComparison.OrdinalIgnoreCase))
        {
            // ECDSA（非對稱式）
            if (string.IsNullOrWhiteSpace(key.PrivateKey))
                throw new ArgumentException("ECDSA 演算法需提供 PrivateKey");

            var ecdsa = ECDsa.Create();
            ecdsa.ImportFromPem(key.PrivateKey.ToCharArray());
            return ecdsa;
        }

        throw new NotSupportedException($"不支援的演算法: {key.Algorithm}");
    }
}