using Infrastructure.Caching.Redis;
using SevenStar.Shared.Domain.Entity.Platform;
using System;
using System.Collections.Generic;
using System.Text;

namespace SevenStar.Shared.Domain.Database;

public class PlatformDb : IPlatformDb
{
    public Task<CompanyRedisDbEntity> GetCompanyRedisDb(int companyId, RedisDbEnum redisDb)
    {
        throw new NotImplementedException();
    }

    public Task<List<CompanyRedisDbEntity>> GetCompanyRedisDb(int companyId)
    {
        throw new NotImplementedException();
    }
}
