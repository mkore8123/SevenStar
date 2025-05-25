using Common.Attributes;
using Common.Enums;
using Common.Utils;
using Infrastructure.Caching.Redis;
using Infrastructure.Data.MySql;
using Infrastructure.Data.Npgsql;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
using SevenStar.Shared.Domain.Database;
using SevenStar.Shared.Domain.DbContext;
using SevenStar.Shared.Domain.Enums;
using SevenStar.Shared.Domain.Extensions;
using SevenStar.Shared.Domain.Redis;
using StackExchange.Redis;
using System.Linq.Expressions;
using System.Reflection;

namespace SevenStar.Shared.Domain.Api.Extensions;

public static class AssemblyMapping
{
    public static readonly string BaseAssemblyName = "SevenStar.Shared.Domain";

    public static readonly Dictionary<DataSource, string> PlatformAssemblies = new()
    {
        { DataSource.MySql, "SevenStar.Data.Platform.MySql" },
        { DataSource.PostgreSql, "SevenStar.Data.Platform.PostgreSql" }
        // 根據實際情況添加其他映射
    };

    public static readonly Dictionary<DataSource, string> BackendAssemblies = new()
    {
        { DataSource.MySql, "SevenStar.Data.Backend.MySql" },
        { DataSource.PostgreSql, "SevenStar.Data.Backend.PostgreSql" }
        // 根據實際情況添加其他映射
    };

    public static readonly Dictionary<DataSource, string> CompanyAssemblies = new()
    {
        { DataSource.MySql, "SevenStar.Data.Company.MySql" },
        { DataSource.PostgreSql, "SevenStar.Data.Company.PostgreSql" }
        // 根據實際情況添加其他映射
    };
}

/// <summary>
/// 提供擴充方法以根據資料庫類型注入對應的 DbContext 與 Repository。
/// </summary>
public static class DbContextExtensions
{
    /// <summary>
    /// 根據資料庫類型取得對應的組件名稱。
    /// </summary>
    /// <param name="map">資料庫類型與組件名稱的映射。</param>
    /// <param name="source">資料庫類型。</param>
    /// <returns>對應的組件名稱。</returns>
    /// <exception cref="NotSupportedException">當指定的資料庫類型未在映射中定義時拋出。</exception>
    private static string GetAssemblyName(Dictionary<DataSource, string> map, DataSource source) =>
        map.TryGetValue(source, out var name)
            ? name
            : throw new NotSupportedException($"在 AssemblyMapping 中找不到 DataSource = {source} 的對應組件名稱。");

    /// <summary>
    /// 根據資料庫類型驗證連線字串的有效性。
    /// </summary>
    /// <param name="db">資料庫類型。</param>
    /// <param name="conn">連線字串。</param>
    /// <returns>若連線字串有效則為 true，否則為 false。</returns>
    private static async Task<bool> ValidateConnAsync(DataSource db, string conn) => db switch
    {
        DataSource.MySql => await MySqlConnectionValidator.ValidateAsync(conn),
        DataSource.PostgreSql => await NpgsqlConnectionValidator.ValidateAsync(conn),
        _ => false
    };

    /// <summary>
    /// 安全地載入指定名稱的組件。
    /// </summary>
    /// <param name="name">組件名稱。</param>
    /// <returns>載入的組件。</returns>
    /// <exception cref="InvalidOperationException">當組件無法載入時拋出。</exception>
    private static Assembly LoadAssemblySafely(string name)
    {
        var loaded = AppDomain.CurrentDomain.GetAssemblies()
                                           .FirstOrDefault(a => a.GetName().Name == name);
        if (loaded != null) return loaded;

        try { return Assembly.Load(name); }
        catch { throw new InvalidOperationException($"無法載入組件「{name}」，請確認 DLL 已複製至輸出目錄。"); }
    }

    /// <summary>
    /// 在指定的組件中註冊指定的工廠類型。
    /// </summary>
    /// <typeparam name="TFactory">工廠介面類型。</typeparam>
    /// <param name="services">服務集合。</param>
    /// <param name="asm">組件。</param>
    /// <param name="lifetime">服務生命週期。</param>
    /// <exception cref="InvalidOperationException">當在組件中找不到對應的實作類型時拋出。</exception>
    public static void RegisterFactory<TFactory>(
        this IServiceCollection services,
        Assembly asm,
        ServiceLifetime lifetime,
        params object[] additionalArgs)
        where TFactory : class
    {
        var type = asm.GetTypes()
                      .FirstOrDefault(t => typeof(TFactory).IsAssignableFrom(t)
                                           && !t.IsAbstract && !t.IsInterface);
        if (type is null)
            throw new InvalidOperationException(
                $"在組件「{asm.FullName}」中找不到 {typeof(TFactory).Name} 的實作類別。");

        services.Add(new ServiceDescriptor(
            typeof(TFactory),
            sp => ActivatorUtilities.CreateInstance(sp, type, additionalArgs),
            lifetime));
    }

    /// <summary>
    /// 根據資料庫類型與連線字串注入平台資料庫相關服務。
    /// </summary>
    /// <param name="services">服務集合。</param>
    /// <param name="databaseType">資料庫類型。</param>
    /// <param name="platformConnectionString">平台資料庫連線字串。</param>
    /// <returns>更新後的服務集合。</returns>
    /// <exception cref="InvalidOperationException">當連線字串無效或無法連線時拋出。</exception>
    public static IServiceCollection AddPlatformDb(
        this IServiceCollection services,
        DataSource databaseType,
        string platformConnectionString)
    {
        if (!ValidateConnAsync(databaseType, platformConnectionString).GetAwaiter().GetResult())
            throw new InvalidOperationException("平台資料庫連線字串無效或無法連線。");

        var asmName = GetAssemblyName(AssemblyMapping.PlatformAssemblies, databaseType);
        var asm = LoadAssemblySafely(asmName);

        services.RegisterFactory<IPlatformDbFactory>(asm, ServiceLifetime.Singleton, platformConnectionString);

        services.AddScoped<IPlatformDb>(sp =>
        {
            var factory = sp.GetRequiredService<IPlatformDbFactory>();
            return factory.CreatePlatformDbAsync().GetAwaiter().GetResult();
        });

        return services;
    }

    /// <summary>
    /// 根據公司 ID 注入公司遊戲資料庫相關服務。
    /// </summary>
    /// <param name="services">服務集合。</param>
    /// <param name="companyId">公司 ID。</param>
    /// <returns>更新後的服務集合。</returns>
    public static IServiceCollection AddCompanyGamesDb(this IServiceCollection services, int companyId)
    {
        foreach (var assemblyKvp in AssemblyMapping.CompanyAssemblies)
        {
            var assembly = LoadAssemblySafely(assemblyKvp.Value);
            RepositoryAutoRegistrar.RegisterFromAssembly<ICompanyGameDbContext>(assembly, assemblyKvp.Key);
        }

        services.TryAddSingleton<ISingletonCacheService, SingletonCacheService>();
        services.AddScoped<IGeneralDbFactory, GeneralDbFactory>();
        services.AddScoped<ICompanyGameDb>(sp =>
        {
            var generalDbFactory = sp.GetRequiredService<IGeneralDbFactory>();
            var companyDb = generalDbFactory.CreateCompanyGameDbAsync(companyId).GetAwaiter().GetResult();

            return companyDb;
        });

        return services;
    }

    /// <summary>
    /// 根據公司 ID 注入公司 Redis 資料庫相關服務。
    /// </summary>
    /// <param name="services">服務集合。</param>
    /// <param name="companyId">公司 ID。</param>
    /// <returns>更新後的服務集合。</returns>
    public static IServiceCollection AddCompanyRedisDb(this IServiceCollection services, int companyId)
    {
        var redisDbList = Enum.GetValues(typeof(RedisDbEnum)).Cast<RedisDbEnum>().ToList();
        
        services.AddSingleton<ICompanyRedisDbFactory>(sp =>
        {
            var platformDb = sp.GetRequiredService<IPlatformDb>();
            return new CompanyRedisDbFactory(platformDb, companyId);
        });

        redisDbList.ForEach(redisDb =>
        {
            services.AddKeyedScoped<IDatabaseAsync>(redisDb, (sp, key) =>
            {
                var factory = sp.GetRequiredService<ICompanyRedisDbFactory>();
                return factory.GetDatabaseAsync((RedisDbEnum)key).GetAwaiter().GetResult();
            });
        });

        return services;
    }

    //public static IServiceCollection AddRepositories(this IServiceCollection services, SiteEnum siteEnum)
    //{
    //    var basedAssembly = Assembly.Load(AssemblyMapping.BaseAssemblyName);
    //    var implementAassemblies = siteEnum switch
    //    {
    //        SiteEnum.Company => 
    //            AssemblyMapping.CompanyAssemblies.Values.Select(assemblyName => Assembly.Load(assemblyName)).ToList(),

    //        _ => throw new KeyNotFoundException($"未找到 SiteEnum = {siteEnum} 的組件映射。")
    //    };

    //    services.AddRepositoriesFromAssembly<ICompanyGameDbContext>(basedAssembly, implementAassemblies);
    //    return services;
    //}

    /// <summary>
    /// 從指定的組件中自動註冊所有繼承指定 DbContext 基底介面的 Repository。
    /// </summary>
    /// <typeparam name="TDbContextBase">DbContext 基底介面類型。</typeparam>
    /// <param name="services">服務集合。</param>
    /// <param name="domainAssembly">包含 DbContext 介面的組件。</param>
    /// <param name="repositoryAssemblies">包含 Repository 實作的組件集合。</param>
    /// <returns>更新後的服務集合。</returns>
    /// <exception cref="InvalidOperationException">當發現重複的 KeyedService 註冊時拋出。</exception>
    //public static IServiceCollection AddRepositoriesFromAssembly<TDbContextBase>(this IServiceCollection services,
    //    Assembly domainAssembly, IEnumerable<Assembly> repositoryAssemblies)
    //    where TDbContextBase : class
    //{
    //    // 取得所有繼承 TDbContextBase 的介面（例如 IUserRepository）
    //    var dbIfaces = domainAssembly.GetTypes()
    //                                 .Where(t => t.IsInterface &&
    //                                             typeof(TDbContextBase).IsAssignableFrom(t) &&
    //                                             t != typeof(TDbContextBase))
    //                                 .ToList();

    //    // 防止同一 interface-key 組合被註冊多次
    //    var registered = new Dictionary<(Type iface, object key), Type>();

    //    foreach (var repoAsm in repositoryAssemblies)
    //    {
    //        foreach (var impl in repoAsm.GetTypes().Where(t => t.IsClass && !t.IsAbstract))
    //        {
    //            var attr = impl.GetCustomAttribute<KeyedServiceAttribute>();
    //            if (attr is null) continue;

    //            var svcIface = impl.GetInterfaces().FirstOrDefault(i => dbIfaces.Contains(i));
    //            if (svcIface is null) continue;

    //            var pair = (svcIface, attr.Key);

    //            if (registered.TryGetValue(pair, out var exist))
    //                throw new InvalidOperationException($"""
    //                KeyedService 重複註冊衝突：
    //                介面：{svcIface.FullName}
    //                Key：{attr.Key}
    //                已存在實作：{exist.FullName}
    //                衝突實作：{impl.FullName}
    //            """);

    //            registered[pair] = impl;

    //            // 使用正確的 Keyed Service 註冊 API（從 .NET 8 起支援）
    //            services.Add(new ServiceDescriptor(
    //                serviceType: svcIface,
    //                serviceKey: attr.Key,
    //                implementationType: impl,
    //                lifetime: attr.Lifetime));
    //        }
    //    }

    //    return services;
    //}
}
