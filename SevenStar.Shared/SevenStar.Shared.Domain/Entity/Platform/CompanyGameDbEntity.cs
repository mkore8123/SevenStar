using SevenStar.Shared.Domain.Database;
using System;
using System.Collections.Generic;
using System.Text;

namespace SevenStar.Shared.Domain.Entity.Platform;

public class CompanyGameDbEntity : IPlatformDbContext
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
    /// 公司遊戲庫連線字串
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;
}
