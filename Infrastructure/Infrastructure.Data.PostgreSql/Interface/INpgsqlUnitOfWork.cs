﻿using Infrastructure.Data.Interface;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Data.PostgreSql.Interface;

public interface INpgsqlUnitOfWork : IUnitOfWork
{
    NpgsqlConnection Connection { get; set; }
}