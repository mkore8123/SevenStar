using Npgsql;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Data.Npgsql;

public interface INpgsqlUnitOfWork : IUnitOfWork
{
    public NpgsqlConnection Connection { get; set; }
}