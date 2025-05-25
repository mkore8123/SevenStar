using SevenStar.Shared.Domain.DbContext.Entity.Platform;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Text;

namespace SevenStar.Shared.Domain.DbContext;

public class SingletonCacheService : ISingletonCacheService
{
    private readonly ConcurrentDictionary<int, Lazy<Task<CompanyGameDbEntity>>> _companyCache = new();

    private readonly ConcurrentDictionary<int, Lazy<Task<CompanyGameDbEntity>>> _backendCache = new();

    public Task<CompanyGameDbEntity> BackendGetOrAddAsync(int backendId, Func<Task<CompanyGameDbEntity>> factory)
    {
        var lazy = _backendCache.GetOrAdd(backendId, _ => new Lazy<Task<CompanyGameDbEntity>>(factory));
        return lazy.Value;
    }

    public Task<CompanyGameDbEntity> CompnayGetOrAddAsync(int companyId, Func<Task<CompanyGameDbEntity>> factory)
    {
        var lazy = _companyCache.GetOrAdd(companyId, _ => new Lazy<Task<CompanyGameDbEntity>>(factory));
        return lazy.Value;
    }
}