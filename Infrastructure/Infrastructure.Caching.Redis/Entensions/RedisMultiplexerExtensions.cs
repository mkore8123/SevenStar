using StackExchange.Redis;
using Infrastructure.Caching.Redis.Interface;

namespace Infrastructure.Caching.Redis.Entensions;

public static class RedisMultiplexerExtensions
{
    /// <summary>
    /// 透過模糊查詢 key 取得所有 Redis Hash 資料，並轉換成 T 強型別物件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="connection"></param>
    /// <param name="database"></param>
    /// <param name="pattern"></param>
    /// <returns></returns>
    public static async Task<Dictionary<string, T>> GetHashByPatternAsync<T>(this IConnectionMultiplexer connection, int database, string pattern)
        where T : IRedisHashInitializable, new()
    {
        var db = connection.GetDatabase(database);

        var lua = @"
        local pattern = ARGV[1]
        local cursor = '0'
        local result = {}

        repeat
            local scan = redis.call('SCAN', cursor, 'MATCH', pattern, 'COUNT', 100)
            cursor = scan[1]
            local keys = scan[2]

            for _, key in ipairs(keys) do
                local hash = redis.call('HGETALL', key)
                table.insert(result, hash)
            end
        until cursor == '0'

        return result
        ";

        var redisResult = await db.ScriptEvaluateAsync(lua, keys: Array.Empty<RedisKey>(), values: new RedisValue[] { pattern });

        var resultArray = (RedisResult[])redisResult!;
        var dict = new Dictionary<string, T>();

        foreach (var item in resultArray)
        {
            var hashData = (RedisResult[])item;

            var hashEntries = new HashEntry[hashData.Length / 2];
            for (int j = 0; j < hashData.Length; j += 2)
            {
                hashEntries[j / 2] = new HashEntry((RedisValue)hashData[j], (RedisValue)hashData[j + 1]);
            }

            var model = new T();
            model.LoadFromHash(hashEntries);

            var key = model.GetRedisKey();
            dict[key] = model;
        }

        return dict;
    }

    /// <summary>
    /// 根據 List<T> 物件一次批次更新 Redis Hash 資料
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="connection"></param>
    /// <param name="database"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    public static async Task HashSetBatchAsync<T>(this IConnectionMultiplexer connection, int database, List<(string key, T model)> items)
        where T : IRedisHashInitializable, new()
    {
        if (items == null || items.Count == 0)
            return;

        var db = connection.GetDatabase();
        var batch = db.CreateBatch();
        var tasks = new List<Task>();

        foreach (var (key, model) in items)
        {
            if (model == null)
                continue;

            var hashEntries = model.ConvertToHash();
            var task = batch.HashSetAsync(key, hashEntries);
            tasks.Add(task);
        }

        batch.Execute();
        await Task.WhenAll(tasks);
    }

    /// <summary>
    /// 取得 Redis Hash 資料，並轉換成 T 強型別物件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="connection"></param>
    /// <param name="database"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public static async Task<T?> HashGetAsync<T>(this IConnectionMultiplexer connection, int database, string key)
        where T : IRedisHashInitializable, new()
    {
        var db = connection.GetDatabase();
        var entries = await db.HashGetAllAsync(key);

        if (entries.Length == 0)
            return default;

        var instance = new T();
        instance.LoadFromHash(entries);

        return instance;
    }

    /// <summary>
    /// 更新單筆 Redis Hash 資料
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="connection"></param>
    /// <param name="database"></param>
    /// <param name="key"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    public static async Task HashSetAsync<T>(this IConnectionMultiplexer connection, int database, string key, T model)
        where T : IRedisHashInitializable, new()
    {
        if (model == null)
            return;

        var db = connection.GetDatabase();
        var hashModel = model.ConvertToHash();

        await db.HashSetAsync(key, hashModel);
    }

    public static async Task HashSetBatchLuaAsync<T>(this IConnectionMultiplexer connection, int database, List<T> items)
        where T : IRedisHashInitializable, new()
    {
        if (items == null || items.Count == 0)
            return;

        var args = new List<RedisValue>();

        foreach (var item in items)
        {
            if (item == null)
                continue;

            var key = item.GetRedisKey();
            var hashEntries = item.ConvertToHash();

            if (hashEntries.Length == 0)
                continue;

            args.Add(key);
            foreach (var entry in hashEntries)
            {
                args.Add(entry.Name.ToString());
                args.Add(entry.Value.ToString());
            }
            args.Add("_END_");
        }

        const string luaScript = @"local i = 1
            while i <= #ARGV do
                local key = ARGV[i]
                i = i + 1

                while i <= #ARGV and ARGV[i] ~= '_END_' do
                    local field = ARGV[i]
                    local value = ARGV[i + 1]

                    if field ~= nil and value ~= nil then
                        redis.call('HSET', key, field, value)
                    end

                    i = i + 2
                end

                i = i + 1 -- skip _END_
            end

            return 'OK'";

        var db = connection.GetDatabase();
        await db.ScriptEvaluateAsync(luaScript, Array.Empty<RedisKey>(), args.ToArray());
    }
}