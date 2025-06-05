using SevenStar.Shared.Domain.DbContext.Platform.Entity;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Text;

namespace SevenStar.Shared.Domain.Service;

public partial class SingletonCacheService : ISingletonCacheService
{
    private readonly ConcurrentDictionary<int, Lazy<Task<CompanyGameDbEntity>>> _companyCache = new();

    private readonly ConcurrentDictionary<int, Lazy<Task<CompanyGameDbEntity>>> _backendCache = new();

    public Task<CompanyGameDbEntity> GetOrAddBackendDbAsync(int backendId, Func<Task<CompanyGameDbEntity>> factory)
    {
        var lazy = _backendCache.GetOrAdd(backendId, _ => new Lazy<Task<CompanyGameDbEntity>>(factory, LazyThreadSafetyMode.ExecutionAndPublication));
        return lazy.Value;
    }

    public Task<CompanyGameDbEntity> GetOrAddCompnayDbAsync(int companyId, Func<Task<CompanyGameDbEntity>> factory)
    {
        var lazy = _companyCache.GetOrAdd(companyId, _ => new Lazy<Task<CompanyGameDbEntity>>(factory, LazyThreadSafetyMode.ExecutionAndPublication));
        return lazy.Value;
    }   
}