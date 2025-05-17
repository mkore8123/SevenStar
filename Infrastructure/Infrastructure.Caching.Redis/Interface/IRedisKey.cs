
namespace Infrastructure.Caching.Redis.Interface;

public interface IRedisKey
{
    /// <summary>
    /// 取得該物件的 Redis Key
    /// </summary>
    /// <returns></returns>
    string GetRedisKey();
}
