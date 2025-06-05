using SevenStar.Shared.Domain.DbContext.Platform.Entity;

namespace SevenStar.Shared.Domain.Service;

/// <summary>
/// 全域單例記憶體快取服務介面，支援公司資料庫實體與 JWT 設定的快取查找。
/// 提供高效 thread-safe 快取機制，避免重複查詢 DB。
/// </summary>
public partial interface ISingletonCacheService
{
    /// <summary>
    /// 取得或建立指定公司專屬的 CompanyGameDbEntity（公司遊戲資料庫連線物件）。
    /// 若快取已存在則直接回傳；若無則執行 factory 方法取得資料並快取。
    /// 用於多租戶遊戲/公司業務，每個公司可綁定獨立資料庫。
    /// </summary>
    /// <param name="companyId">公司主鍵 id（唯一）</param>
    /// <param name="factory">產生 CompanyGameDbEntity 的非同步工廠方法</param>
    /// <returns>指定公司對應的 CompanyGameDbEntity 實例</returns>
    Task<CompanyGameDbEntity> GetOrAddCompnayDbAsync(int companyId, Func<Task<CompanyGameDbEntity>> factory);

    /// <summary>
    /// 取得或建立指定後台專屬的 CompanyGameDbEntity（後台資料庫連線物件）。
    /// 若快取已存在則直接回傳；若無則執行 factory 方法取得資料並快取。
    /// 用於平台後台、管理端等後台業務。
    /// </summary>
    /// <param name="backendId">後台主鍵 id（唯一）</param>
    /// <param name="factory">產生 CompanyGameDbEntity 的非同步工廠方法</param>
    /// <returns>指定後台對應的 CompanyGameDbEntity 實例</returns>
    Task<CompanyGameDbEntity> GetOrAddBackendDbAsync(int backendId, Func<Task<CompanyGameDbEntity>> factory);
}