using System;
using System.Collections.Generic;
using System.Text;

namespace SevenStar.Shared.Domain.DbContext.Platform.Entity;

/// <summary>
/// 公司主檔 Entity，對應資料表 company
/// </summary>
public class CompanyEntity
{
    /// <summary>
    /// 公司主鍵，自動編號，唯一識別每一家公司（對應 company.id）
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 公司代碼（唯一），如 companyA、companyB。系統內唯一識別（對應 company.code）
    /// </summary>
    public string Code { get; set; } = default!;

    /// <summary>
    /// 公司名稱（中文或識別名稱）（對應 company.name）
    /// </summary>
    public string Name { get; set; } = default!;

    /// <summary>
    /// 是否啟用：true=正常，false=已關閉（對應 company.is_active。可軟刪除/備查）
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// 建立時間（UTC 時區）（對應 company.created_at）
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
