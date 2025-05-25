
namespace SevenStar.Shared.Domain.Database;

public interface ICompanyGameDbFactory
{
    /// <summary>
    /// 創建公司遊戲 DB，若查詢不到則直接拋出例外，實作方法須確認執行緒安全，能支援平行處理
    /// </summary>
    /// <param name="companyId">公司id</param>
    /// <returns></returns>
    Task<ICompanyGameDb> CreateCompanyGameDbAsync(int companyId);

    /// <summary>
    /// 直接建立透過提供的連線字串建立 CompanyGameDb，並且不由 CompanyGameDbFactory 維護併發管理
    /// </summary>
    /// <param name="baackendId"></param>
    /// <param name="companyId"></param>
    /// <param name="connectionString"></param>
    /// <returns></returns>
    Task<ICompanyGameDb> CreateCompanyGameDbAsync(int baackendId, int companyId, string connectionString);
}
