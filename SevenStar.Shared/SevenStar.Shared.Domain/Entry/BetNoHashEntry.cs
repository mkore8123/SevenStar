using Infrastructure.Caching.Redis.Interface;
using SevenStar.Shared.Domain.Enums;
using StackExchange.Redis;

namespace SevenStar.Shared.Domain.Entry;

public class BetNoHashEntry : IRedisHashInitializable
{
    public int BetNo { get; set; } = 0;

    /// <summary>
    /// 投注號碼小分類
    /// </summary>
    public BetNoTypeEnum BetNoType { get; set; } 

    /// <summary>
    /// 投注號碼大分類
    /// </summary>
    public BetNoCategoryEnum BetNoCategory { get; set; } 

    public BetNoHashEntry(int betNo)
    {
        BetNo = betNo;
    }

    public void LoadFromHash(HashEntry[] entries)
    {
        foreach (var entry in entries)
        {
            var value = entry.Value.ToString();
            if (string.IsNullOrEmpty(value))
                continue;

            switch (entry.Name.ToString())
            {
                case "BetNo":
                    BetNo = int.Parse(entry.Value!);
                    break;
                case "BetNoType":
                    BetNoType = Enum.Parse<BetNoTypeEnum>(entry.Value!);
                    break;
                case "BetNoCategory":
                    BetNoCategory = Enum.Parse<BetNoCategoryEnum>(entry.Value!);
                    break;
            }
        }
    }
    public HashEntry[] ConvertToHash()
    {
        var model = new HashEntry[]
        {
                new HashEntry("BetNo", BetNo.ToString()),
                new HashEntry("BetNoType", BetNoType.ToString()),
                new HashEntry("BetNoCategory", BetNoCategory.ToString())
        };
        return model;
    }

    public string GetRedisKey()
    {
        return $"RealHold:{BetNoCategory}:{BetNoType}:{BetNo}";
    }
}
