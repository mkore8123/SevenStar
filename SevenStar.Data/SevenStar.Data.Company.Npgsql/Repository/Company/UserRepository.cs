using Dapper;
using Npgsql;
using System.Data;
using Common.Enums;
using Common.Attributes;
using Microsoft.Extensions.DependencyInjection;
using SevenStar.Shared.Domain.Entity.Company;
using SevenStar.Shared.Domain.Repository;


namespace SevenStar.Data.Company.Npgsql.Repository.Company;

[KeyedService(DataSource.Npgsql, ServiceLifetime.Scoped)]
public class UserRepository : IUserRepository
{
    private NpgsqlConnection Connection { get; }

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

    public async Task<int> CreateAsync(UserEntity entity, IDbTransaction? transaction = null)
    {
        var sql = new SqlBuilder();
        var abc = sql.AddTemplate("");

        var result = await Connection.ExecuteAsync(abc.RawSql, transaction);
        return result;
    }
}