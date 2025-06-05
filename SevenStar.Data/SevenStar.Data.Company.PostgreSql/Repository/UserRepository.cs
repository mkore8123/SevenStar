using Dapper;
using Npgsql;
using System.Data;
using Common.Enums;
using Common.Attributes;
using Microsoft.Extensions.DependencyInjection;
using SevenStar.Shared.Domain.DbContext.Company.Repository;
using SevenStar.Shared.Domain.DbContext.Company.Entity;


namespace SevenStar.Data.Company.PostgreSql.Repository;

[KeyedService(DataSource.PostgreSql, ServiceLifetime.Scoped)]
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
        var user1 = users.ToList();
        return user1;
    }

    public async Task<UserEntity> GetAsync(long id)
    {
        var sql = "SELECT \"Id\", \"Name\" FROM public.\"User\" /**where**/";

        var user = await Connection.QueryFirstAsync<UserEntity>(sql);
        return user;
    }

    public async Task<int> CreateAsync(UserEntity entity, IDbTransaction? transaction = null)
    {
        throw new NotImplementedException("CreateAsync method is not implemented.");
        //var sql = new SqlBuilder();
        //var abc = sql.AddTemplate("");

        //var result = await Connection.ExecuteAsync(abc.RawSql, transaction);
        //return result;
    }
}