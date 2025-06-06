﻿using Npgsql;
using SevenStar.Shared.Domain.DbContext.Entity.Platform;
using SevenStar.Shared.Domain.DbContext.Platform.Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SevenStar.Data.Company.PostgreSql.Repository.Platform;

public class CompanyRepository : ICompanyRepository
{
    private NpgsqlConnection Connection { get; }

    public CompanyRepository(NpgsqlConnection connection)
    {
        Connection = connection;
    }

    public Task<int> Create(CompanyEntity entity, IDbTransaction? transaction = null)
    {
        throw new NotImplementedException();
    }

    public Task<List<CompanyEntity>> GetAsync()
    {
        throw new NotImplementedException();
    }

    public Task<CompanyEntity> GetAsync(long id)
    {
        throw new NotImplementedException();
    }
}
