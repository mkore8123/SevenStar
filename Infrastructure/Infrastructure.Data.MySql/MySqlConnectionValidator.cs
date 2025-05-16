using MySqlConnector; 

namespace Infrastructure.Data.MySql;

public class MySqlConnectionValidator
{
    /// <summary>
    /// 驗證 MySQL 連線字串格式與可連線性
    /// </summary>
    /// <param name="connectionString">要驗證的連線字串</param>
    /// <param name="checkConnectivity">是否要進一步嘗試連線</param>
    /// <param name="timeoutSeconds">連線逾時秒數（若驗證連線時）</param>
    /// <returns>是否驗證成功</returns>
    public static async Task<bool> ValidateAsync(string connectionString, bool checkConnectivity = false, int timeoutSeconds = 3)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            return false;

        MySqlConnectionStringBuilder? builder = null;

        // 格式驗證
        try
        {
            builder = new MySqlConnectionStringBuilder(connectionString);

            if (string.IsNullOrWhiteSpace(builder.Server) ||
                builder.Port <= 0 ||
                string.IsNullOrWhiteSpace(builder.UserID))
            {
                return false;
            }
        }
        catch
        {
            return false;
        }

        if (!checkConnectivity)
            return true;

        try
        {
            builder.ConnectionTimeout = (uint)timeoutSeconds;

            await using var conn = new MySqlConnection(builder.ConnectionString);
            await conn.OpenAsync();
            return conn.State == System.Data.ConnectionState.Open;
        }
        catch
        {
            return false;
        }
    }
}
