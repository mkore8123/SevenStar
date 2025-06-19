using Common.Api.Authen.Jwt.Model;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Api.Authen.Jwt.Interface;

/// <summary>
/// JWT 簽章金鑰提供者介面（JWS 專用）。<br/>
/// 提供根據發行者與接收者取得現役可用的簽章金鑰，或依據指定 KeyId 取得歷史簽章金鑰，供 JWT Token 的簽章與驗證流程使用。
/// </summary>
public interface IJwtSigningKeyProvider
{
    /// <summary>
    /// 取得指定 issuer/audience 組合下「目前可用的現役簽章金鑰」（通常為最新版本），用於簽發 JWT Token（JWS）。
    /// </summary>
    /// <param name="issuer">JWT Token 的發行者（iss），例如公司代碼、平台識別字串。</param>
    /// <param name="audience">JWT Token 的接收者（aud），例如 mobile、web、api-client 等。</param>
    /// <returns>
    /// 若查無對應可用金鑰，則拋出例外或回傳 null（依實作決定）；
    /// 否則回傳現役的 <see cref="JwtSigningKey"/> 金鑰，用於產生 JWS Token。
    /// </returns>
    Task<JwtSigningKey> GetAvailableKeyAsync(string issuer, string audience);

    /// <summary>
    /// 取得指定 issuer/audience/kid 的歷史簽章金鑰，用於驗證 JWT Token（JWS）簽章正確性。
    /// </summary>
    /// <param name="issuer">JWT Token 的發行者（iss）。</param>
    /// <param name="audience">JWT Token 的接收者（aud）。</param>
    /// <param name="keyId">JWT Header 中指定的 Key ID（kid），用於對應簽章金鑰。</param>
    /// <returns>
    /// 若查無對應金鑰，則拋出例外或回傳 null（依實作決定）；
    /// 否則回傳符合指定組合的 <see cref="JwtSigningKey"/> 實例。
    /// </returns>
    Task<JwtSigningKey> GetKeyAsync(string issuer, string audience, string keyId);
}
