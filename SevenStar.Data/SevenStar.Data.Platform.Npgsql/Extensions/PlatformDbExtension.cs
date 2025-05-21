
using Microsoft.Extensions.DependencyInjection;
using SevenStar.Shared.Domain.Database;

namespace SevenStar.Data.Platform.Npgsql.Extensions;

/// <summary>
/// 
/// </summary>
public static class PlatformDbExtension
{
    /// <summary>
    /// 注入 PostgreSql NpgsqlDataSource，支援進階配置
    /// </summary>
    public static IServiceCollection AddPlatformDbHandler(this IServiceCollection services, int companyId, string platformConnectionString)
    {
        //services.AddSingleton<IPlatformDb>(serviceProvider =>
        //{
        //    return new CompanyGameDbFactory(serviceProvider, platformConnectionString);
        //});

        //services.AddScoped<IPlatformDb>(serviceProvider =>
        //{
        //    var factory = serviceProvider.GetService<ICompanyGameDbFactory>();
        //    var companyGameDb = factory?.CreateCompanyGameDbAsync(companyId).Result;
            
        //    return companyGameDb!;
        //});

        return services;
    }
}