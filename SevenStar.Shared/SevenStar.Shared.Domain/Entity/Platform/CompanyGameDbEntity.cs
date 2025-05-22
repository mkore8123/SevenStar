using System;
using System.Collections.Generic;
using System.Text;

namespace SevenStar.Shared.Domain.Entity.Platform;

public class CompanyGameDbEntity
{
    /// <summary>
    /// 公司遊戲庫連線字串
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;
}
