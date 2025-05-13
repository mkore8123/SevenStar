using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Infrastructure.Data.Npgsql;

public interface INpgsqlRepository<T>
{
    /// <summary>
    /// PostgreSQL 的原始連線
    /// </summary>
    public NpgsqlConnection Connection { get; }

    /// <summary>
    /// 當前交易（可選）
    /// </summary>
    public NpgsqlTransaction? Transaction { get; set; }
}


