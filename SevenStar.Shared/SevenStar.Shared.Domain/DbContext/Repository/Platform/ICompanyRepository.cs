using System.Data;
using SevenStar.Shared.Domain.Database;
using SevenStar.Shared.Domain.DbContext.Entity.Platform;

namespace SevenStar.Shared.Domain.DbContext.Repository.Platform;

public interface ICompanyRepository : IPlatformDbContext
{
    /// <summary>
    /// 取得所有 Company 物件
    /// </summary>
    /// <returns></returns>
    Task<List<CompanyEntity>> GetAsync();

    /// <summary>
    /// 取得指定 Company 物件
    /// </summary>
    /// <returns></returns>
    Task<CompanyEntity> GetAsync(long id);

    /// <summary>
    /// 建立 Company 物件
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="transaction"></param>
    /// <returns></returns>
    Task<int> Create(CompanyEntity entity, IDbTransaction? transaction = null);
}
