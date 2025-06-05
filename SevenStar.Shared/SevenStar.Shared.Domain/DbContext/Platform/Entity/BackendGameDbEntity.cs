using Common.Enums;

namespace SevenStar.Shared.Domain.DbContext.Platform.Entity;

public class BackendGameDbEntity
{
    /// <summary>
    /// 總控id
    /// </summary>
    public int BackendId { get; set; } = -1;

    /// <summary>
    /// 所使用的資料庫類型
    /// </summary>
    public DataSource DataSource { get; set; }

    /// <summary>
    /// 總控遊戲庫連線字串
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;
}
