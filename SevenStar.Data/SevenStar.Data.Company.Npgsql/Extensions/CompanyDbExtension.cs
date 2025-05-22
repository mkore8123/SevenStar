using SevenStar.Shared.Domain.Database;
using Microsoft.Extensions.DependencyInjection;

namespace SevenStar.Data.Company.Npgsql.Extensions;

/// <summary>
/// 
/// </summary>
public static class CompanyDbExtension
{
    /// <summary>
    /// 注入 PostgreSql NpgsqlDataSource，支援進階配置
    /// </summary>
    public static IServiceCollection AddCompanyDbHandler(this IServiceCollection services, int companyId, string platformConnectionString)
    {
        services.AddSingleton<ICompanyGameDbFactory, CompanyGameDbFactory>();

        services.AddScoped<ICompanyGameDb>(serviceProvider =>
        {
            var factory = serviceProvider.GetService<ICompanyGameDbFactory>();
            var companyGameDb = factory?.CreateCompanyGameDbAsync(companyId).GetAwaiter().GetResult();
            
            return companyGameDb!;
        });

        return services;
    }
}