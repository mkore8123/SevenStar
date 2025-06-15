using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Api.Authen.Jwt.Model;

/// <summary>
/// JWT 快取條目的組合 Key，用於區分 JWT 的簽發與驗證快取項目。<br/>
/// 組成為：Issuer + Audience [+ KeyId (kid)]，支援不含 kid 的「簽發用途」與含 kid 的「驗證用途」兩種模式。<br/>
/// <para>
/// - 簽發用：由 Issuer + Audience 組成，代表一組目前可用的 JWT 設定與簽章/加密金鑰。<br/>
/// - 驗證用：由 Issuer + Audience + Kid 組成，代表一組歷史簽章/加密金鑰。
/// </para>
/// <para>此型別通常作為快取字典的 Key 使用，例如 <c>ConcurrentDictionary&lt;JwtKey, JwtTokenConfig&gt;</c>。</para>
/// </summary>
public readonly record struct JwtKey(string Issuer, string Audience, string? Kid = null)
{
    /// <summary>
    /// 轉換為字串格式，格式為："Issuer::Audience" 或 "Issuer::Audience::Kid"。
    /// </summary>
    /// <returns>字串化的 Key 值。</returns>
    public override string ToString() =>
        Kid == null ? $"{Issuer}::{Audience}" : $"{Issuer}::{Audience}::{Kid}";

    /// <summary>
    /// 建立簽發用的 <see cref="JwtKey"/>，不包含 kid（用於目前可用的 JWT 設定與金鑰快取）。
    /// </summary>
    /// <param name="issuer">Token 發行者（iss）</param>
    /// <param name="audience">Token 接收者（aud）</param>
    /// <returns>用於簽發用途的 <see cref="JwtKey"/></returns>
    public static JwtKey ForIssue(string issuer, string audience) => new(issuer, audience);

    /// <summary>
    /// 從 <see cref="JwtTokenConfig"/> 建立簽發用的 <see cref="JwtKey"/>。
    /// </summary>
    /// <param name="config">JWT 設定物件</param>
    /// <returns>用於簽發用途的 <see cref="JwtKey"/></returns>
    public static JwtKey ForIssue(JwtTokenConfig config) =>
        new(config.Issuer, config.Audience);

    /// <summary>
    /// 建立驗證用的 <see cref="JwtKey"/>，包含 kid。
    /// </summary>
    /// <param name="issuer">Token 發行者（iss）</param>
    /// <param name="audience">Token 接收者（aud）</param>
    /// <param name="kid">JWT Header 中的 Key ID（kid）</param>
    /// <returns>用於驗證用途的 <see cref="JwtKey"/></returns>
    public static JwtKey ForValidate(string issuer, string audience, string kid) =>
        new(issuer, audience, kid);

    /// <summary>
    /// 從 <see cref="JwtTokenConfig"/> 和 <see cref="JwtSigningKey"/> 建立驗證用的 <see cref="JwtKey"/>。
    /// </summary>
    /// <param name="config">JWT 設定物件</param>
    /// <param name="key">簽章金鑰物件</param>
    /// <returns>用於驗證用途的 <see cref="JwtKey"/></returns>
    public static JwtKey ForValidate(JwtTokenConfig config, JwtSigningKey key) =>
        new(config.Issuer, config.Audience, key.KeyId);

    /// <summary>
    /// 從 <see cref="JwtTokenConfig"/> 和 <see cref="JwtEncryptingKey"/> 建立驗證用的 <see cref="JwtKey"/>。
    /// </summary>
    /// <param name="config">JWT 設定物件</param>
    /// <param name="key">加密金鑰物件</param>
    /// <returns>用於驗證用途的 <see cref="JwtKey"/></returns>
    public static JwtKey ForValidate(JwtTokenConfig config, JwtEncryptingKey key) =>
        new(config.Issuer, config.Audience, key.KeyId);
}
