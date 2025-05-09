using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Attributes;

/// <summary>
/// 定義注入 FromKeyedServices 的服務
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class KeyedServiceAttribute : Attribute
{
    public Enum Key { get; }
    public ServiceLifetime Lifetime { get; }

    public KeyedServiceAttribute(object key, ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        if (key is not Enum enumKey)
            throw new ArgumentException("Key must be an enum value.", nameof(key));

        Key = enumKey;
        Lifetime = lifetime;
    }
}

