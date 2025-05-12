using Infrastructure.Data.Npgsql;
using Npgsql;
using SevenStar.Data.Company.Nppgsql.Repository;
using SevenStar.Shared.Domain;
using SevenStar.Shared.Domain.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace SevenStar.Data.Company.Nppgsql;

public class CompanyGameDb : NpgsqlUnitOfWork, ICompanyGameDb
{
    private readonly Dictionary<Type, object> _repoCache = new();

    public CompanyGameDb(IServiceProvider provider, NpgsqlDataSource dataSource) : base(provider, dataSource)
    {
    }

    public TRepository GetRepository<TRepository>() where TRepository : class
    {
        var type = typeof(TRepository);
        if (_repoCache.TryGetValue(type, out var existing))
            return (TRepository)existing;

        // 建立對應的實例（你可依介面判斷對應哪個實作）
        object repo = type switch
        {
            var t when t == typeof(IUserRepository) => new UserRepository(Cconnection, Transaction),
            _ => throw new NotSupportedException($"Repository type not registered: {type.FullName}")
        };

        _repoCache[type] = repo;
        return (TRepository)repo;
    }
}
