using Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SevenStar.Shared.Domain.Extensions;

public static class RepositoryFactoryMap
{
    private static readonly Dictionary<(Type Interface, DataSource Source), Func<object, object>> _map = new();

    public static bool Contains((Type, DataSource) key) => _map.ContainsKey(key);

    internal static void RegisterInternal(Type iface, DataSource source, Func<object, object> factory)
    {
        _map[(iface, source)] = factory;
    }

    public static TInterface Create<TInterface>(DataSource source, object connection)
        where TInterface : class
    {
        var key = (typeof(TInterface), source);
        if (!_map.TryGetValue(key, out var factory))
            throw new NotSupportedException($"未註冊 {typeof(TInterface).Name} 的 {source} 實作");

        return (TInterface)factory(connection);
    }
}