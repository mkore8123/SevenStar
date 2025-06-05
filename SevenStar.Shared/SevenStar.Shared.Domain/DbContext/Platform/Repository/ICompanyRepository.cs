using System.Data;
using SevenStar.Shared.Domain.DbContext.Platform.Entity;

namespace SevenStar.Shared.Domain.DbContext.Platform.Repository;

/// <summary>
/// 公司主檔 Repository 介面，提供公司資料的 CRUD 操作
/// </summary>
public interface ICompanyRepository : IPlatformDbContext
{
    /// <summary>
    /// 新增公司
    /// </summary>
    /// <param name="company">公司資料</param>
    /// <returns>資料庫產生的公司主鍵 id</returns>
    int Create(CompanyEntity company);

    /// <summary>
    /// 依主鍵查詢公司
    /// </summary>
    /// <param name="id">公司 id</param>
    /// <returns>公司物件，若查無則回傳 null</returns>
    CompanyEntity? GetById(int id);

    /// <summary>
    /// 依公司代碼查詢公司
    /// </summary>
    /// <param name="code">公司代碼（唯一）</param>
    /// <returns>公司物件，若查無則回傳 null</returns>
    CompanyEntity? GetByCode(string code);

    /// <summary>
    /// 查詢所有公司
    /// </summary>
    /// <returns>公司物件清單</returns>
    IEnumerable<CompanyEntity> GetAll();

    /// <summary>
    /// 更新公司資料（依 id 覆寫）
    /// </summary>
    /// <param name="company">公司物件（需帶主鍵 id）</param>
    void Update(CompanyEntity company);

    /// <summary>
    /// 刪除公司（依主鍵 id）
    /// </summary>
    /// <param name="id">公司主鍵 id</param>
    void Delete(int id);
}
