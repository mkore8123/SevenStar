using Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SevenStar.Shared.Domain.DbContext.Entity.Platform;

public class CompanyGameDbEntity
{
    /// <summary>
    /// 總控id
    /// </summary>
    public int BackendId { get; set; } = -1;

    /// <summary>
    /// 公司id
    /// </summary>
    public int CompanyId { get; set; } = -1;

    /// <summary>
    /// 所使用的資料庫類型
    /// </summary>
    public DataSource DataSource { get; set; }

    /// <summary>
    /// 公司遊戲庫連線字串
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;
}
