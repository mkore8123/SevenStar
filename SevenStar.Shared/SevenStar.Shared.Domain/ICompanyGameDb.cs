using Infrastructure.Data.Npgsql.Interface;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Text;

namespace SevenStar.Shared.Domain;

public interface ICompanyGameDb : INpgsqlUnitOfWork
{
    /// <summary>
    /// 公司id
    /// </summary>
    int CompanyId { get;  }

    /// <summary>
    /// 使用目前當前的 ConnectionString 建立新的連線物件; 主要用於平行處理避免使用同一個連線物件造成錯誤
    /// </summary>
    /// <returns></returns>
    Task<ICompanyGameDb> CreateNewInstanceAsync();

    /// <summary>
    /// 取得指定的 Repository 物件
    /// </summary>
    /// <typeparam name="TRepository"></typeparam>
    /// <returns></returns>
    TRepository GetRepository<TRepository>() where TRepository : class;
}