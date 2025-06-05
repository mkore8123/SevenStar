using Common.Api.Auth.Jwt;

namespace SevenStar.Shared.Domain.Api;

public partial interface IApiSingletonCacheService
{
    /// <summary>
    /// 取得或建立指定 issuer/audience 組合的 JWT Token 配置（用於簽發 Token）。
    /// 若快取已存在則直接回傳；若無則執行 factory 方法查詢資料並快取。
    /// 此快取僅依 issuer + audience 做分組，不區分 kid。
    /// 簽發 Token 通常僅用預設現役金鑰組合。
    /// </summary>
    /// <param name="issuer">JWT Token 的發行者（iss）</param>
    /// <param name="audience">JWT Token 的目標受眾（aud）</param>
    /// <param name="factory">查詢 JwtTokenConfig 的非同步工廠方法</param>
    /// <returns>指定組合對應的 JwtTokenConfig 設定實例</returns>
    Task<JwtTokenConfig> GetOrAddJwtConfigForIssueAsync(string issuer, string audience, Func<Task<JwtTokenConfig>> factory);

    /// <summary>
    /// 取得或建立指定 issuer/audience/kid 組合的 JWT Token 配置（用於 Token 驗證）。
    /// 若快取已存在則直接回傳；若無則執行 factory 方法查詢資料並快取。
    /// 此快取依 issuer + audience + kid 完全分組，確保驗證 token 時可查到歷史金鑰版本。
    /// </summary>
    /// <param name="issuer">JWT Token 的發行者（iss）</param>
    /// <param name="audience">JWT Token 的目標受眾（aud）</param>
    /// <param name="kid">JWT Token 的金鑰編號（kid）</param>
    /// <param name="factory">查詢 JwtTokenConfig 的非同步工廠方法</param>
    /// <returns>指定組合對應的 JwtTokenConfig 設定實例</returns>
    Task<JwtTokenConfig> GetOrAddJwtConfigForValidateAsync(string issuer, string audience, string kid, Func<Task<JwtTokenConfig>> factory);
}
