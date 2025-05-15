using SevenStar.Shared.Domain.Entity.Company;
using SevenStar.Shared.Domain.Entity.Platform;
using System.Data;

namespace SevenStar.Shared.Domain.Repository;

public interface ICompanyRepository
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
