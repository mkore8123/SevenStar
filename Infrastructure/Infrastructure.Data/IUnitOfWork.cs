using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Infrastructure.Data;

public interface IUnitOfWork : IDisposable, IAsyncDisposable
{
    /// <summary>
    /// 執行交易程序
    /// </summary>
    /// <param name="operation"></param>
    /// <returns></returns>
    Task ExecuteAsync(Func<IDbTransaction, Task> operation);
}
