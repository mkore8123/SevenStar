using SevenStar.Shared.Domain.DbContext.Entity.Platform;
using System;
using System.Collections.Generic;
using System.Text;

namespace SevenStar.Shared.Domain.DbContext;

public interface ISingletonCacheService
{
    Task<CompanyGameDbEntity> GetOrAddCompnayDbAsync(int companyId, Func<Task<CompanyGameDbEntity>> factory);

    Task<CompanyGameDbEntity> GetOrAddBackendDbAsync(int backendId, Func<Task<CompanyGameDbEntity>> factory);
}