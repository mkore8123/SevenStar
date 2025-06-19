using Common.Api.Authen.Jwt.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Common.Api.Authen.Jwt.Interface;

/// <summary>
/// 專責處理 JWT Token 相關快取項目的服務介面。
/// 包含簽發與驗證時所需的 Token 配置、簽章金鑰、加密金鑰。
/// </summary>
public interface IJwtCacheService
{
    /// <summary>
    /// 取得指定 issuer/audience 組合的 JWT Token 設定（僅簽發用，不含 kid）。
    /// </summary>
    JwtTokenConfig? GetJwtConfigForIssue(string issuer, string audience);

    /// <summary>
    /// 取得指定 issuer/audience 組合的 JWS 簽章金鑰（用於簽發）。
    /// </summary>
    JwtSigningKey? GetJwsSigningForIssue(string issuer, string audience);

    /// <summary>
    /// 取得指定 issuer/audience 組合的 JWE 加密金鑰（用於簽發）。
    /// </summary>
    JwtEncryptingKey? GetJweEncryptingForIssue(string issuer, string audience);

    /// <summary>
    /// 取得指定 issuer/audience/kid 的 JWS 金鑰（用於驗證）。
    /// </summary>
    JwtSigningKey? GetJwsSigningForValidate(string issuer, string audience, string kid);

    /// <summary>
    /// 取得指定 issuer/audience/kid 的 JWE 金鑰（用於驗證）。
    /// </summary>
    JwtEncryptingKey? GetJweEncryptingForValidate(string issuer, string audience, string kid);

    /// <summary>
    /// 更新整批 JWT 快取資料（通常由背景服務觸發）。
    /// </summary>
    bool RefreshJwtConfig(ConcurrentDictionary<JwtKey, JwtTokenConfig> newerJwtConfigIssue,
        ConcurrentDictionary<JwtKey, JwtSigningKey> newerjwsConfigValidate,
        ConcurrentDictionary<JwtKey, JwtEncryptingKey> newerjweConfigValidate);
}
