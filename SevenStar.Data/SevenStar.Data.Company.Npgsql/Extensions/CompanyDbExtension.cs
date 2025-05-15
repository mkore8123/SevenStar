using Microsoft.Extensions.DependencyInjection;
using SevenStar.Shared.Domain;
using System;

namespace SevenStar.Data.Company.Npgsql.Extensions;

/// <summary>
/// 
/// </summary>
public static class CompanyDbExtension
{
    /// <summary>
    /// 注入 PostgreSql NpgsqlDataSource，支援進階配置
    /// </summary>
    public static IServiceCollection AddNpgSqlHandler(this IServiceCollection services, string npgSqlConnectionString)
    {
        services.AddSingleton<ICompanyGameDbFactory>(serviceProvider =>
        {
            var platformConnectionString = "";
            return new CompanyGameDbFactory(serviceProvider, platformConnectionString);
        });

        services.AddSingleton<ICompanyGameDb>(serviceProvider =>
        {
            var currentCompanyId = 1;
            var companyGameDbFactory = serviceProvider.GetService<ICompanyGameDbFactory>();
            var companyGameDb = companyGameDbFactory.CreateCompanyGameDbAsync(currentCompanyId).Result;

            return companyGameDb;
        });

        return services;
    }
}