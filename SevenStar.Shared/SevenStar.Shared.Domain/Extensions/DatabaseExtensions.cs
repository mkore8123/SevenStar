using Common.Enums;
using Microsoft.Extensions.DependencyInjection;
using SevenStar.Shared.Domain.Database;
using SevenStar.Shared.Domain.RedisCache;
using System.Reflection;

namespace SevenStar.Shared.Domain.Api.Extensions;

public static class DatabaseExtensions
{
    /// <summary>
    /// 根據選則的資料庫類型，注入對應平台的 DbContext
    /// </summary>
    public static IServiceCollection AddPlatformDb(this IServiceCollection services, DataSource databaseType, string platformConnectionString)
    {
        // 根據 enum 取得對應組件名稱
        var assemblyName = databaseType switch
        {
            DataSource.MySql => "SevenStar.Data.Platform.MySql",
            DataSource.PostgreSql => "SevenStar.Data.Platform.Npgsql",
            _ => throw new NotSupportedException($"不支援的資料庫類型: {databaseType}")
        };

        // 嘗試載入 Assembly
        var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == assemblyName)
            ?? Assembly.Load(assemblyName); // 若未載入則主動載入

        // 掃描符合 ICompanyGameDbFactory 的具體實作
        var factoryType = assembly.GetTypes()
            .FirstOrDefault(t =>
                typeof(IPlatformDbFactory).IsAssignableFrom(t) &&
                !t.IsAbstract && !t.IsInterface);

        if (factoryType == null)
            throw new InvalidOperationException($"在組件 {assemblyName} 中找不到 ICompanyGameDbFactory 的實作類別");

        // 註冊 IPlatformDbFactory 為 Singleton
        services.AddSingleton(typeof(IPlatformDbFactory), factoryType);

        services.AddSingleton<IPlatformDb>(serviceProvider =>
        {
            var factory = serviceProvider.GetRequiredService<IPlatformDbFactory>();
            var platformDb = factory.CreatePlatformDbAsync().GetAwaiter().GetResult();

            return platformDb;
        });

        return services;
    }

    /// <summary>
    /// 根據選則的資料庫類型，注入對應公司的 DbContext
    /// </summary>
    public static IServiceCollection AddCompanyGameDb(this IServiceCollection services, DataSource databaseType, int companyId)
    {
        // 根據 enum 取得對應組件名稱
        var assemblyName = databaseType switch
        {
            DataSource.MySql => "SevenStar.Data.Company.MySql",
            DataSource.PostgreSql => "SevenStar.Data.Company.Npgsql",
            _ => throw new NotSupportedException($"不支援的資料庫類型: {databaseType}")
        };

        // 嘗試載入 Assembly
        var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == assemblyName)
            ?? Assembly.Load(assemblyName); // 若未載入則主動載入

        // 掃描符合 ICompanyGameDbFactory 的具體實作
        var factoryType = assembly.GetTypes()
            .FirstOrDefault(t =>
                typeof(ICompanyGameDbFactory).IsAssignableFrom(t) &&
                !t.IsAbstract && !t.IsInterface);

        if (factoryType == null)
            throw new InvalidOperationException($"在組件 {assemblyName} 中找不到 ICompanyGameDbFactory 的實作類別");

        // 註冊 ICompanyGameDbFactory 為 Singleton
        services.AddSingleton(typeof(ICompanyGameDbFactory), factoryType);

        // 註冊 ICompanyGameDb，根據公司 ID 建立實例
        services.AddScoped<ICompanyGameDb>(serviceProvider =>
        {
            var factory = serviceProvider.GetRequiredService<ICompanyGameDbFactory>();
            var companyGameDb = factory.CreateCompanyGameDbAsync(companyId).GetAwaiter().GetResult();
            return companyGameDb;
        });

        return services;
    }

    /// <summary>
    /// 取得指定公司的 RedisDb 連線物件
    /// </summary>
    /// <param name="services"></param>
    /// <param name="companyId"></param>
    /// <returns></returns>
    public static IServiceCollection AddCompanyRedisDb(this IServiceCollection services, int companyId)
    {
        services.AddSingleton<ICompanyRedisDbFactory>(sp =>
        {
            var platformDb = sp.GetRequiredService<IPlatformDb>();
            return new CompanyRedisDbFactory(platformDb, companyId);
        });

        return services;
    }
}