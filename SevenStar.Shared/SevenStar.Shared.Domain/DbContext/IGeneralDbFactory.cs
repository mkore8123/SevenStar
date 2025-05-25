using SevenStar.Shared.Domain.Database;
using System;
using System.Collections.Generic;
using System.Text;

namespace SevenStar.Shared.Domain.DbContext;

public partial interface IGeneralDbFactory
{
    /// <summary>
    /// 根據公司 ID 創建公司遊戲資料庫實例。
    /// </summary>
    /// <param name="companyId"></param>
    /// <returns></returns>
    Task<ICompanyGameDb> CreateCompanyGameDbAsync(int companyId);
}
