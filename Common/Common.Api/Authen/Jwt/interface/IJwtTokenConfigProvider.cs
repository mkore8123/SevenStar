using Common.Api.Auth.Jwt;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Common.Api.Authen.Jwt.@interface;

/// <summary>
/// 提供 JWT Token 設定（<see cref="JwtTokenConfig"/>）的取得機制介面，支援依據模型產生或依據 Token 內容解構對應配置。
/// 適用於多租戶、多組 JWT 配置（如不同 issuer、audience、keyId），可於發行與驗證流程動態取得正確配置。
/// </summary>
/// <typeparam name="TModel">代表 JWT 產生時的應用模型類型（如使用者、租戶資訊等）。</typeparam>
public interface IJwtTokenConfigProvider<TModel>
{
    /// <summary>
    /// 依據指定的模型（如使用者、租戶、裝置資訊等），取得發行 JWT 所需的 <see cref="JwtTokenConfig"/> 配置。
    /// 常見於產生 Token 流程，根據模型資訊動態決定要用哪一組 JWT 設定（如 issuer、audience、kid 等）。
    /// </summary>
    /// <param name="model">應用於產生 Token 的模型實例，通常包含租戶/公司、裝置、身分等資訊。</param>
    /// <returns>對應的 <see cref="JwtTokenConfig"/> 配置實例。</returns>
    Task<JwtTokenConfig> GetForModelAsync(TModel model);

    /// <summary>
    /// 根據已產生的 JWT Token 字串，解構出其中的 issuer(iss)、audience(aud)、keyId(kid) 等欄位，
    /// 以動態取得正確的 <see cref="JwtTokenConfig"/> 配置，用於 Token 驗證或解密流程。
    /// </summary>
    /// <param name="token">JWT Token 字串（通常為三段 base64url 編碼的結構）。</param>
    /// <returns>解析後對應的 <see cref="JwtTokenConfig"/> 配置實例。</returns>
    Task<JwtTokenConfig> GetForTokenAsync(string token);
}