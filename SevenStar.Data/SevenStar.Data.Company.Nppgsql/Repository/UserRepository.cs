using Common.Attributes;
using Common.Enum;
using Dapper;
using Infrastructure.Data.Npgsql;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using SevenStar.Shared.Domain.Entity;
using SevenStar.Shared.Domain.Repository;
using System.Transactions;


namespace SevenStar.Data.Company.Nppgsql.Repository;

[KeyedService(DataSource.Npgsql, ServiceLifetime.Scoped)]
public class UserRepository : INpgsqlRepository<IUserRepository>, IUserRepository
{
    public NpgsqlConnection Connection { get; }

    public NpgsqlTransaction? Transaction { get; set; } = null;

    public UserRepository(NpgsqlConnection connection, NpgsqlTransaction? transaction)
    {
        Connection = connection;
        Transaction = transaction;
    }

    public async Task<List<UserEntity>> GetUsersAsync()
    {
        var sql = "SELECT \"Id\", \"Name\" FROM public.\"User\" /**where**/";


        var users = await Connection.QueryAsync<UserEntity>(sql, transaction: Transaction);
        return users.ToList();
    }
}