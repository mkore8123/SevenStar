using System.Reflection;
using Common.Attributes;
using Microsoft.Extensions.DependencyInjection;


namespace Common.Extensions;

/// <summary>
/// Api 注入 Serilog 的擴充方法
/// </summary>
public static class RegisterServiceExtension
{
    /// <summary>
    /// 配置注入 Key Service 的服務
    /// </summary>
    /// <param name="services"></param>
    /// <param name="assembly"></param>
    /// <returns></returns>
    /// <exception cref="NotSupportedException"></exception>
    public static IServiceCollection RegisterAssemblyHandling(this IServiceCollection services, Assembly assembly)
    {
        var typesWithAttribute = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract)
            .Select(t => new
            {
                ImplementationType = t,
                Attribute = t.GetCustomAttribute<KeyedServiceAttribute>(),
                ServiceTypes = t.GetInterfaces()
            })
            .Where(x => x.Attribute != null);

        foreach (var item in typesWithAttribute)
        foreach (var serviceType in item.ServiceTypes)
        {
            switch (item.Attribute.Lifetime)
            {
                case ServiceLifetime.Singleton:
                    services.AddKeyedSingleton(serviceType, item.Attribute.Key, item.ImplementationType);
                    break;
                case ServiceLifetime.Scoped:
                    services.AddKeyedScoped(serviceType, item.Attribute.Key, item.ImplementationType);
                    break;
                case ServiceLifetime.Transient:
                    services.AddKeyedTransient(serviceType, item.Attribute.Key, item.ImplementationType);
                    break;
                default:
                    throw new NotSupportedException($"Unsupported ServiceLifetime: {item.Attribute.Lifetime}");
            }
        }

        return services;
    }
}