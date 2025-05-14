using System;
using System.Collections.Generic;
using System.Text;

namespace SevenStar.Shared.Domain;

public interface ICompanyGameDbFactory
{
    /// <summary>
    /// 創建公司遊戲 DB
    /// </summary>
    /// <param name="companyId">公司id</param>
    /// <returns></returns>
    ICompanyGameDb CreateCompanyGameDb(int companyId);

    /// <summary>
    /// 創建公司遊戲 DB
    /// </summary>
    /// <param name="connectionString">連線字串</param>
    /// <returns></returns>
    ICompanyGameDb CreateCompanyGameDb(string connectionString);
}
