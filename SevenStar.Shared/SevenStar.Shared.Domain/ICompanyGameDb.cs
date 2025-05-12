using Infrastructure.Data.Npgsql;
using System;
using System.Collections.Generic;
using System.Text;

namespace SevenStar.Shared.Domain;

public interface ICompanyGameDb : INpgsqlUnitOfWork
{
    TRepository GetRepository<TRepository>() where TRepository : class;
}
