using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Api.Authen.Jwt.Interface;

/// <summary>
/// 提供 JWT 簽章金鑰與演算法取得機制的介面。
/// 支援多租戶、多裝置與金鑰版本（KeyId, kid）動態解析。
/// 典型情境：根據 issuer（發行者）、audience（接收者/裝置）與 keyId（kid，金鑰版本）取得對應金鑰與演算法。
/// </summary>
public interface IJwtSigningKeyProvider
{
    /// <summary>
    /// 取得指定 issuer（發行者）、audience（接收者/裝置）及 keyId（kid）對應的 <see cref="SecurityKey"/> 實例。
    /// 用於 JWT 簽章驗證與產生。
    /// </summary>
    /// <param name="issuer">Token 發行者（iss），通常為公司/租戶代號。</param>
    /// <param name="audience">Token 接收者（aud），通常為應用系統或裝置類型（如 mobile, web）。</param>
    /// <param name="keyId">金鑰唯一標識（kid），可用於金鑰輪替或多版本金鑰管理。</param>
    /// <returns>對應的 <see cref="SecurityKey"/> 實例。若無對應，應拋出例外或回傳 null。</returns>
    Task<SecurityKey> GetKeyAsync(string issuer, string audience, string keyId);

    /// <summary>
    /// 取得指定 issuer、audience 及 keyId 對應金鑰的簽章演算法（如 HmacSha256、RS256）。
    /// 通常需與 <see cref="GetKey"/> 回傳之金鑰型態配對。
    /// </summary>
    /// <param name="issuer">Token 發行者（iss）。</param>
    /// <param name="audience">Token 接收者（aud）。</param>
    /// <param name="keyId">金鑰唯一標識（kid）。</param>
    /// <returns>對應的 JWT 簽章演算法名稱（如 SecurityAlgorithms.HmacSha256）。</returns>
    Task<string> GetAlgorithmAsync(string issuer, string audience, string keyId);
}