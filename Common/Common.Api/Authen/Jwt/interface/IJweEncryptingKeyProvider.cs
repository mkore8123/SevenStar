using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Api.Authen.Jwt.@interface;

public interface IJweEncryptingKeyProvider
{
    /// <summary>
    /// 取得加密用 EncryptingCredentials（適用於 JWE 加密）
    /// </summary>
    Task<object?> GetEncryptingCredentialsAsync(string issuer, string audience, string? keyId);

    /// <summary>
    /// 取得解密用 SecurityKey（適用於 JWE 解密）
    /// </summary>
    Task<SecurityKey?> GetDecryptingKeyAsync(string issuer, string audience, string? keyId);

    /// <summary>
    /// 取得演算法資訊（alg, enc）
    /// </summary>
    Task<(string Algorithm, string ContentAlgorithm)?> GetAlgorithmsAsync(string issuer, string audience, string? keyId);
}
