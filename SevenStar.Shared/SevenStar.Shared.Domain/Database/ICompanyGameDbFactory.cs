
namespace SevenStar.Shared.Domain.Database;

public interface ICompanyGameDbFactory
{
    /// <summary>
    /// 創建公司遊戲 DB，若查詢不到則直接拋出例外，實作方法須確認執行緒安全，能支援平行處理
    /// </summary>
    /// <param name="companyId">公司id</param>
    /// <returns></returns>
    Task<ICompanyGameDb> CreateCompanyGameDbAsync(int companyId);
}
