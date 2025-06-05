using Microsoft.IdentityModel.Tokens;

namespace Common.Api.Auth.Jwt;

/// <summary>
/// 動態提供多組 JWT 驗證參數（<see cref="TokenValidationParameters"/>）的解析介面。
/// 支援依據傳入的 JWT Token 動態取得對應的驗證設定，適用於多租戶、多 issuer/audience/kid 等場景。
/// </summary>
public interface IMultiJwtValidationConfigProvider
{
    /// <summary>
    /// 依據指定的 JWT Token 字串，解析出正確的 <see cref="TokenValidationParameters"/> 設定。
    /// 
    /// <para>
    /// 實作時通常會先解構 token，取得其 issuer（iss）、audience（aud）、keyId（kid）等欄位，
    /// 再根據欄位內容動態選擇對應的驗證參數（如簽章金鑰、演算法、允許的發行者/接收者、token 有效時間等）。
    /// </para>
    /// </summary>
    /// <param name="token">JWT Token 字串（Base64Url 格式，通常以 "Bearer " header 帶入）。</param>
    /// <returns>用於該 token 驗證的 <see cref="TokenValidationParameters"/> 實例。</returns>
    Task<TokenValidationParameters> GetValidationParameters(string token);
}