using System;
using System.Collections.Generic;
using System.Text;

namespace SevenStar.Shared.Domain.Database;

public interface IPlatformDbFactory
{
    /// <summary>
    /// 創建平台資料庫
    /// </summary>
    /// <param name="companyId">公司id</param>
    /// <returns></returns>
    Task<IPlatformDb> CreatePlatformDbAsync();
}
