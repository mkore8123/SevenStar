using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Transactions;

namespace Infrastructure.Data;

public interface IBaseRepository<T>
{
    T Set(IDbTransaction? transaction = null)
    {
        Transaction = transaction;

        return (T)this;
    }
}
