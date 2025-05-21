using System;
using System.Collections.Generic;
using System.Text;

namespace SevenStar.Shared.Domain.Entity.Platform;

public class CompanyRedisDbEntity : IPlatoformDb
{
    public string RedisConnectionString { get; set; } = string.Empty;
}
