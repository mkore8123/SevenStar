using Common.Api.Authen.Jwt.Model;

namespace Common.Api.Authen.Jwt.Interface;

/// <summary>
/// JWT 加密金鑰提供者介面（JWE 專用）。<br/>
/// 提供根據發行者與接收者取得現役可用的加密金鑰，或依據指定 KeyId 取得歷史金鑰，用於 JWT 加密（JWE）流程。
/// </summary>
public interface IJweEncryptingKeyProvider
{
    /// <summary>
    /// 取得指定 issuer/audience 組合下「目前可用的現役加密金鑰」（通常為最新版本），用於加密 JWT Token（JWE）。
    /// </summary>
    /// <param name="issuer">JWT Token 的發行者（iss），通常為公司代號、網址或識別字串。</param>
    /// <param name="audience">JWT Token 的接收者（aud），例如 mobile、web、partner-api 等。</param>
    /// <returns>
    /// 若查無對應加密金鑰，則回傳 null；
    /// 否則回傳指定 issuer/audience 組合對應的現役加密金鑰 <see cref="JwtEncryptingKey"/> 實例。
    /// </returns>
    Task<JwtEncryptingKey?> GetAvailableKeyAsync(string issuer, string audience);

    /// <summary>
    /// 取得指定 issuer/audience/kid 的歷史加密金鑰，用於驗證或解密 JWT Token（JWE）時對應使用。
    /// </summary>
    /// <param name="issuer">JWT Token 的發行者（iss）。</param>
    /// <param name="audience">JWT Token 的接收者（aud）。</param>
    /// <param name="keyId">JWT Header 中指定的 Key ID（kid），用來對應特定加密金鑰。</param>
    /// <returns>
    /// 若查無對應金鑰，則回傳 null；
    /// 否則回傳符合指定組合的 <see cref="JwtEncryptingKey"/> 金鑰資料。
    /// </returns>
    Task<JwtEncryptingKey?> GetKeyAsync(string issuer, string audience, string keyId);
}