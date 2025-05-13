using Npgsql;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Data.Npgsql;

public interface INpgsqlUnitOfWork : IUnitOfWork
{
    public NpgsqlConnection Cconnection { get; set; }

    public NpgsqlTransaction? Transaction { get; set; }

    Task ExecuteAsync(Func<NpgsqlTransaction, Task> operation);
}