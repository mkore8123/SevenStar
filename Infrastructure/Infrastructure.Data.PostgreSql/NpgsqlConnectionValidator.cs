using Npgsql;

namespace Infrastructure.Data.PostgreSql;

public class NpgsqlConnectionValidator
{
    /// <summary>
    /// 驗證 PostgreSQL 連線字串格式與可連線性
    /// </summary>
    /// <param name="connectionString">要驗證的連線字串</param>
    /// <param name="checkConnectivity">是否要進一步嘗試連線</param>
    /// <param name="timeoutSeconds">連線逾時秒數（若驗證連線時）</param>
    /// <returns>是否驗證成功</returns>
    public static async Task<bool> ValidateAsync(string connectionString, bool checkConnectivity = false, int timeoutSeconds = 3)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            return false;

        // 格式驗證
        NpgsqlConnectionStringBuilder? builder = null;
        try
        {
            builder = new NpgsqlConnectionStringBuilder(connectionString);
            if (string.IsNullOrWhiteSpace(builder.Host) ||
                builder.Port <= 0 ||
                string.IsNullOrWhiteSpace(builder.Username))
            {
                return false;
            }
        }
        catch
        {
            return false;
        }

        // 是否要進一步驗證連線
        if (!checkConnectivity)
            return true;

        try
        {
            builder.Timeout = timeoutSeconds;

            await using var conn = new NpgsqlConnection(builder.ConnectionString);
            await conn.OpenAsync();
            return conn.State == System.Data.ConnectionState.Open;
        }
        catch
        {
            return false;
        }
    }
}
