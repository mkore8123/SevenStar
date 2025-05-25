using SevenStar.Shared.Domain.Database;

namespace SevenStar.Shared.Domain.DbContext.Entity.Platform;

public class CompanyRedisDbEntity
{
    public string RedisConnectionString { get; set; } = string.Empty;
}
