using Infrastructure.Data.Npgsql;
using System;
using System.Collections.Generic;
using System.Text;

namespace SevenStar.Shared.Domain;

public interface ICompanyGameDb : INpgsqlUnitOfWork
{
    /// <summary>
    /// 取得註冊的 Repository
    /// </summary>
    /// <typeparam name="TRepository"></typeparam>
    /// <returns></returns>
    TRepository GetRepository<TRepository>() where TRepository : class;
}