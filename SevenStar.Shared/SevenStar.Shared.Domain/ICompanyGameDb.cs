using Infrastructure.Data.Npgsql.Interface;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Text;

namespace SevenStar.Shared.Domain;

/// <summary>
/// 公司遊戲資料庫操作功能
/// </summary>
public interface ICompanyGameDb : INpgsqlUnitOfWork
{
    /// <summary>
    /// 所屬總控id
    /// </summary>
    int BackendId { get; }

    /// <summary>
    /// 本身公司id
    /// </summary>
    int CompanyId { get;  }

    /// <summary>
    /// 創建新實例的資料庫連線存取資源，避免與預設的連線物件相同，造成在平行處理使用相同連線物件造成衝突
    /// 通常用於 Parallel.ForEach 或 Task.Run 平行處理，又要使用到連線物件時使用
    /// </summary>
    /// <returns></returns>
    Task<ICompanyGameDb> CreateInstanceAsync();

    /// <summary>
    /// 取得指定的 Repository 物件
    /// </summary>
    /// <typeparam name="TRepository"></typeparam>
    /// <returns></returns>
    TRepository GetRepository<TRepository>() where TRepository : class;
}