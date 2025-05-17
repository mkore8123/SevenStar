using System.Data;

namespace SevenStar.Shared.Domain.Service;

public interface IUserService
{
    /// <summary>
    /// 準備欲執行建立 User 的非同步交易包方法，留給上層決定合適時機使用
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    Task<Func<IDbTransaction, Task>> PrepareCreateAsync(string name);
}
