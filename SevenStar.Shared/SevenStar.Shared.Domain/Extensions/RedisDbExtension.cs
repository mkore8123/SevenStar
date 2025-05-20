using Microsoft.Extensions.DependencyInjection;
using SevenStar.Shared.Domain.Database;
using SevenStar.Shared.Domain.RedisCache;

namespace SevenStar.Shared.Domain.Extensions;

public static class RedisDbExtension
{
    public static IServiceCollection AddCompanyRedisDatabases(this IServiceCollection services, int companyId)
    {
        services.AddSingleton<ICompanyRedisDbFactory>(sp =>
        {
            var platformDb = sp.GetRequiredService<IPlatformDb>();
            return new CompanyRedisDbFactory(platformDb, companyId);
        });

        return services;
    }
}
