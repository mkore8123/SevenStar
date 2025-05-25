using SevenStar.Shared.Domain.DbContext.Entity.Platform;
using System;
using System.Collections.Generic;
using System.Text;

namespace SevenStar.Shared.Domain.DbContext;

public interface ISingletonCacheService
{
    Task<CompanyGameDbEntity> CompnayGetOrAddAsync(int companyId, Func<Task<CompanyGameDbEntity>> factory);

    Task<CompanyGameDbEntity> BackendGetOrAddAsync(int backendId, Func<Task<CompanyGameDbEntity>> factory);
}