using Common.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;

namespace SevenStar.Shared.Domain.Service.Extensions;

public static class DomainServiceExtension
{
    /// <summary>
    /// 掃描並註冊 <c>SevenStar.Shared.Domain.Service</c> 組件中所有服務實作類別至 DI 容器，
    /// 會自動註冊所有名稱以 <c>"Service"</c> 結尾的介面類型。
    /// 若實作類別上標記有 <see cref="KeyedServiceAttribute"/>，則使用 Keyed DI 註冊；
    /// 否則採用傳統方式（例如 <c>AddScoped</c>）註冊介面與其實作。
    /// </summary>
    /// <param name="services">要註冊服務的 <see cref="IServiceCollection"/> 實例。</param>
    /// <returns>更新後的 <see cref="IServiceCollection"/> 實例，支援鏈式呼叫。</returns>
    public static IServiceCollection AddDomainKeyedServices(this IServiceCollection services)
    {
        var assembly = Assembly.Load("SevenStar.Shared.Domain.Service");
        return services.RegisterAllKeyedServicesFromAssembly(assembly, iface => iface.Name.EndsWith("Service"));
    }
}
