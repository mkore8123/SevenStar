using SevenStar.Shared.Domain.Entity;
using System.Data;

namespace SevenStar.Shared.Domain.Repository;

public interface IUserRepository
{
    /// <summary>
    /// 取得所有 User 物件
    /// </summary>
    /// <returns></returns>
    Task<List<UserEntity>> GetAsync();

    /// <summary>
    /// 取得指定 User 物件
    /// </summary>
    /// <returns></returns>
    Task<UserEntity> GetAsync(long id);

    /// <summary>
    /// 建立 User 物件
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="transaction"></param>
    /// <returns></returns>
    Task<int> Create(UserEntity entity, IDbTransaction? transaction = null);
}
