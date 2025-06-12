using Common.Api.Auth.Jwt;
using SevenStar.Shared.Domain.DbContext.Platform.Entity;
using System.Collections.Concurrent;

namespace SevenStar.Shared.Domain.Api;

public partial interface IApiSingletonCacheService
{
    /// <summary>
    /// 取得或建立指定 issuer/audience 組合的 JWS（簽章型）JWT Token 配置（用於簽發 Token）。
    /// 僅適用於 JWS Token 簽發流程（即簽章型 JWT，header.alg ≠ "none"、無加密）。<br/>
    /// 若快取已存在則直接回傳；若無則執行 factory 方法查詢資料並快取。
    /// 此快取僅依 issuer + audience 分組，不區分 kid，
    /// 因簽發 Token 時通常只採用預設現役金鑰組合（即預設 kid）。
    /// </summary>
    /// <param name="issuer">JWT Token 的發行者（iss）</param>
    /// <param name="audience">JWT Token 的目標受眾（aud）</param>
    /// <param name="factory">查詢 JWS 專用 JwtTokenConfig 的非同步工廠方法</param>
    /// <returns>指定組合對應的 JwtTokenConfig 設定實例（JWS專用）</returns>
    Task<JwtTokenConfig> GetOrAddJwsConfigForIssueAsync(string issuer, string audience, Func<Task<JwtTokenConfig>> factory);

    /// <summary>
    /// 取得或建立指定 issuer/audience/kid 組合的 JWS（簽章型）JWT Token 配置（用於 Token 驗證）。
    /// 僅適用於 JWS 驗證流程（即 JWT 為簽章型、header.alg ≠ "none"、無加密）場景。<br/>
    /// 若快取已存在則直接回傳；若無則執行 factory 方法查詢資料並快取。
    /// 此快取依 issuer + audience + kid 完全分組，確保驗證 Token 時可查到所有歷史金鑰版本，
    /// 並支援金鑰輪替及多版本驗證需求。
    /// </summary>
    /// <param name="issuer">JWT Token 的發行者（iss）</param>
    /// <param name="audience">JWT Token 的目標受眾（aud）</param>
    /// <param name="kid">JWT Token 的簽章金鑰編號（kid）</param>
    /// <param name="factory">查詢 JWS 專用 JwtTokenConfig 的非同步工廠方法</param>
    /// <returns>指定組合對應的 JwtTokenConfig 設定實例（JWS專用）</returns>
    Task<JwtTokenConfig> GetOrAddJwsConfigForValidateAsync(string issuer, string audience, string kid, Func<Task<JwtTokenConfig>> factory);

    /// <summary>
    /// 取得指定 issuer/audience 組合的 JWS（簽章型）JWT Token 配置（用於簽發 Token）。<br/>
    /// 僅適用於 JWS Token 簽發流程（即簽章型 JWT，header.alg ≠ "none"、無加密）。
    /// 若快取存在則回傳，否則回傳 null，不會自動執行查詢或建立。
    /// </summary>
    /// <param name="issuer">JWT Token 的發行者（iss）</param>
    /// <param name="audience">JWT Token 的目標受眾（aud）</param>
    /// <returns>指定組合對應的 JwtTokenConfig 設定實例（JWS專用），查無則為 null</returns>
    Task<JwtTokenConfig?> GetJwsConfigForIssueAsync(string issuer, string audience);

    /// <summary>
    /// 取得指定 issuer/audience/kid 組合的 JWS（簽章型）JWT Token 配置（用於 Token 驗證）。<br/>
    /// 僅適用於 JWS 驗證流程（即 JWT 為簽章型、header.alg ≠ "none"、無加密）。
    /// 若快取存在則回傳，否則回傳 null，不會自動執行查詢或建立。
    /// </summary>
    /// <param name="issuer">JWT Token 的發行者（iss）</param>
    /// <param name="audience">JWT Token 的目標受眾（aud）</param>
    /// <param name="kid">JWT Token 的簽章金鑰編號（kid）</param>
    /// <returns>指定組合對應的 JwtTokenConfig 設定實例（JWS專用），查無則為 null</returns>
    Task<JwtTokenConfig?> GetJwsConfigForValidateAsync(string issuer, string audience, string kid);

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    bool RefreshJwtConfig(ConcurrentDictionary<string, Lazy<Task<JwtTokenConfig>>> newerJwtConfigIssue, 
        ConcurrentDictionary<string, Lazy<Task<JwtTokenConfig>>> newerjwtConfigValidate);
}
