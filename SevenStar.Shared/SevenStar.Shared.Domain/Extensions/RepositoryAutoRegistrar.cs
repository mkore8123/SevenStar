using Common.Enums;
using SevenStar.Shared.Domain.Database;
using System.Linq.Expressions;
using System.Reflection;

namespace SevenStar.Shared.Domain.Extensions;

public static class RepositoryAutoRegistrar
{
    public static void RegisterFromAssembly<IDbContext>(Assembly assembly, DataSource source)
    {
        var allTypes = assembly.GetTypes();

        foreach (var type in allTypes)
        {
            // 跳過抽象類別與非 class
            if (!type.IsClass || type.IsAbstract)
                continue;

            // 找出實作 IDbContext 的介面（如 IUserRepository）
            var interfaces = type.GetInterfaces()
                .Where(i => typeof(IDbContext).IsAssignableFrom(i) &&
                            i != typeof(IDbContext))
                .ToList();

            if (interfaces.Count == 0)
                continue;

            // 限制：建構子必須只有一個參數（如 NpgsqlConnection）
            var ctor = type.GetConstructors()
                .FirstOrDefault(c => c.GetParameters().Length == 1);

            if (ctor == null)
                continue;

            var param = ctor.GetParameters()[0];
            var paramType = param.ParameterType;

            // 建立建構委派：Func<object, object>
            var input = Expression.Parameter(typeof(object), "conn");
            var castParam = Expression.Convert(input, paramType);
            var newExpr = Expression.New(ctor, castParam);
            var castResult = Expression.Convert(newExpr, typeof(object));
            var lambda = Expression.Lambda<Func<object, object>>(castResult, input).Compile();

            foreach (var iface in interfaces)
            {
                var key = (iface, source);

                if (RepositoryFactoryMap.Contains(key))
                {
                    Console.WriteLine($"⚠️ 跳過註冊：介面 {iface.FullName} 已在 {source} 註冊過，實作類別為：{type.FullName}");
                    continue;
                }

                RepositoryFactoryMap.RegisterInternal(iface, source, lambda);
                Console.WriteLine($"✅ 已註冊 Repository：{iface.FullName} -> {type.FullName}（DataSource: {source}）");
            }
        }
    }
}