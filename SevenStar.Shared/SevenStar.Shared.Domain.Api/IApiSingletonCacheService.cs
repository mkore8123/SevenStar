using Common.Api.Authen.Jwt.Interface;

namespace SevenStar.Shared.Domain.Api;

/// <summary>
/// 提供 JWT Token 簽發與驗證所需配置的快取服務介面。
/// 此介面為 Singleton 模式，負責管理 JWT 的配置、簽章金鑰、加密金鑰等資訊，
/// 並支援外部服務更新內容，避免重複查詢資料庫或設定來源。
/// </summary>
public partial interface IApiSingletonCacheService
{
    IJwtCacheService Jwt { get; }
}
