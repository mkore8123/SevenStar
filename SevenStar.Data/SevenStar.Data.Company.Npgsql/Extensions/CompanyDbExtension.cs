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
        services.AddSingleton<ICompanyGameDbFactory>(serviceProvider =>
        {
            return new CompanyGameDbFactory(serviceProvider, platformConnectionString);
        });

        services.AddScoped<ICompanyGameDb>(serviceProvider =>
        {
            var currentCompanyId = 1;
            var factory = serviceProvider.GetService<ICompanyGameDbFactory>();
            var companyGameDb = factory?.CreateCompanyGameDbAsync(currentCompanyId).Result;
            
            return companyGameDb!;
        });

        return services;
    }
}