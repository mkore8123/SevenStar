using Microsoft.Extensions.DependencyInjection;
using SevenStar.Shared.Domain.Database;
using System;
using System.Collections.Generic;
using System.Text;

namespace SevenStar.Shared.Domain.DbContext;

/// <summary>
/// 路由器：根據公司 ID 找出所需的資料庫類型，並呼叫對應的 Factory 來產出 ICompanyGameDb
/// </summary>
public class GeneralDbFactory
{
    private readonly IServiceProvider _provider;
    private readonly IPlatformDb _platformDb;

    public GeneralDbFactory(IServiceProvider provider, IPlatformDb platformDb)
    {
        _provider = provider;
        _platformDb = platformDb;
    }

    //public async Task<ICompanyGameDb> CreateBackendGameDbAsync(int backendId)
    //{
    //    if (backendId <= 0)
    //        throw new ArgumentOutOfRangeException(nameof(backendId), "總控 ID 無效。");

    //    // 查詢平台資料，取得該公司使用的資料庫類型
    //    var backendDb = await _platformDb.GetBackendGameDb(backendId);
    //    var dbType = backendDb.DataSource; // DataSource 是 enum: MySql / PostgreSql / SqlServer 等

    //    // 利用 KeyedService 根據 dbType 解析對應的 Factory
    //    var factory = _provider.GetRequiredKeyedService<IBackendGameDbFactory>(dbType);

    //    return await factory.CreateBackendGameDbAsync(backendId);
    //}

    public async Task<ICompanyGameDb> CreateCompanyGameDbAsync(int companyId)
    {
        if (companyId <= 0)
            throw new ArgumentOutOfRangeException(nameof(companyId), "公司 ID 無效。");

        // 查詢平台資料，取得該公司使用的資料庫類型
        var companyDb = await _platformDb.GetCompanyGameDb(companyId);     

        // 利用 KeyedService 根據 dbType 解析對應的 Factory
        var factory = _provider.GetRequiredKeyedService<ICompanyGameDbFactory>(companyDb.DataSource);

        return await factory.CreateCompanyGameDbAsync(companyId);
    }
}
