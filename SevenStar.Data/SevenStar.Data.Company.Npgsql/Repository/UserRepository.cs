using Common.Attributes;
using Common.Enums;
using Dapper;
using Infrastructure.Data.Npgsql;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using SevenStar.Shared.Domain.Entity;
using SevenStar.Shared.Domain.Repository;
using System.Data;


namespace SevenStar.Data.Company.Npgsql.Repository;

[KeyedService(DataSource.Npgsql, ServiceLifetime.Scoped)]
public class UserRepository : INpgsqlRepository, IUserRepository
{
    public NpgsqlConnection Connection { get; }

    public UserRepository(NpgsqlConnection connection)
    {
        Connection = connection;
    }

    public async Task<List<UserEntity>> GetAsync()
    {
        var sql = "SELECT \"Id\", \"Name\" FROM public.\"User\" ";

        var users = await Connection.QueryAsync<UserEntity>(sql);
        return users.ToList();
    }

    public async Task<UserEntity> GetAsync(long id)
    {
        var sql = "SELECT \"Id\", \"Name\" FROM public.\"User\" /**where**/";

        var user = await Connection.QueryFirstAsync<UserEntity>(sql);
        return user;
    }

    public async Task<int> Create(UserEntity entity, IDbTransaction? transaction = null)
    {
        var sql = new SqlBuilder();
        var abc = sql.AddTemplate("");

        var result = await Connection.ExecuteAsync(abc.RawSql, transaction);
        return result;
    }
}