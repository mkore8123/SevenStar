using Common.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SevenStar.Shared.Domain.Service.Extensions;

public static class AutoRegisterExtensions
{
    /// <summary>
    /// 掃描指定組件中所有實作 <typeparamref name="TService"/> 的類別，
    /// 並根據其是否標記 <see cref="KeyedServiceAttribute"/> 進行相對應的 DI 註冊。
    /// 若有標記 <see cref="KeyedServiceAttribute"/>，則使用 Keyed DI 註冊方式（例如 <c>AddKeyedScoped</c>）；
    /// 若無標記，則退回使用傳統的 <c>AddScoped</c> 註冊方式。
    /// </summary>
    /// <typeparam name="TService">目標服務介面類型，僅會註冊實作此介面的類別。</typeparam>
    /// <param name="services">要註冊服務的 <see cref="IServiceCollection"/> 實例。</param>
    /// <param name="assembly">要掃描的組件，例如 <c>Assembly.Load("MyApp.Services")</c>。</param>
    public static void RegisterKeyedServicesFromAssembly<TService>(this IServiceCollection services,
        Assembly assembly)
    {
        var serviceType = typeof(TService);

        var candidates = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && serviceType.IsAssignableFrom(t));

        foreach (var type in candidates)
        {
            var attr = type.GetCustomAttribute<KeyedServiceAttribute>();
            services.RegisterService(serviceType, type, attr);
        }
    }

    /// <summary>
    /// 從指定的 <see cref="Assembly"/> 中自動註冊所有實作介面的類別到 DI 容器中。
    /// 若類別上有 <see cref="KeyedServiceAttribute"/>，則以 Keyed Service 方式註冊；
    /// 否則採用傳統 DI 註冊方式（例如 <c>AddScoped</c>）。
    /// </summary>
    /// <param name="services">要註冊服務的 <see cref="IServiceCollection"/>。</param>
    /// <param name="assembly">要掃描類別的目標組件，例如 <c>Assembly.Load("MyProject.Services")</c>。</param>
    /// <param name="interfaceFilter">
    /// 可選的介面過濾器，用於限定哪些介面會被註冊。<br/>
    /// 例如：<c>t => t.Name.EndsWith("Service")</c> 僅註冊名稱結尾為 "Service" 的介面。<br/>
    /// 若為 <c>null</c> 則所有介面皆會註冊。
    /// </param>
    /// <returns>回傳更新後的 <see cref="IServiceCollection"/>，可用於鏈式呼叫。</returns>
    public static IServiceCollection RegisterAllKeyedServicesFromAssembly(this IServiceCollection services,
        Assembly assembly, Func<Type, bool>? interfaceFilter = null)
    {
        foreach (var type in assembly.GetTypes())
        {
            if (!type.IsClass || type.IsAbstract)
                continue;

            var interfaces = type.GetInterfaces();
            if (interfaceFilter != null)
                interfaces = interfaces.Where(interfaceFilter).ToArray();

            if (!interfaces.Any())
                continue;

            var attr = type.GetCustomAttribute<KeyedServiceAttribute>();

            foreach (var iface in interfaces)
            {
                services.RegisterService(iface, type, attr);
            }
        }

        return services;
    }

    /// <summary>
    /// 根據 <see cref="KeyedServiceAttribute"/> 註冊指定的服務類型到 DI 容器中。
    /// 若有指定 <see cref="KeyedServiceAttribute"/>，則使用 Keyed Service 註冊方式；
    /// 若未指定，則採用傳統的 <c>AddScoped</c> 註冊方式（可重複註冊但不會覆蓋）。
    /// </summary>
    /// <param name="services">DI 容器的 <see cref="IServiceCollection"/>。</param>
    /// <param name="serviceInterface">要註冊的介面類型，例如 <c>typeof(IUserService)</c>。</param>
    /// <param name="implementationType">實作該介面的實體類別，例如 <c>UserService</c>。</param>
    /// <param name="attr">
    /// 指定的 <see cref="KeyedServiceAttribute"/>，若為 <c>null</c> 則使用傳統註冊。
    /// 包含注入 Key 值與生命週期（Scoped、Singleton、Transient）。
    /// </param>
    public static void RegisterService(this IServiceCollection services, Type serviceInterface, Type implementationType,
        KeyedServiceAttribute? attr)
    {
        if (attr != null)
        {
            switch (attr.Lifetime)
            {
                case ServiceLifetime.Singleton:
                    services.AddKeyedSingleton(serviceInterface, attr.Key, implementationType);
                    break;
                case ServiceLifetime.Scoped:
                    services.AddKeyedScoped(serviceInterface, attr.Key, implementationType);
                    break;
                case ServiceLifetime.Transient:
                    services.AddKeyedTransient(serviceInterface, attr.Key, implementationType);
                    break;
            }
        }
        else
        {
            // 傳統方式註冊
            services.TryAdd(ServiceDescriptor.Describe(serviceInterface, implementationType, ServiceLifetime.Scoped));
        }
    }
}
