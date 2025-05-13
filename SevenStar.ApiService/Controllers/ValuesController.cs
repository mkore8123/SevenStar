
using Dapper;
using Infrastructure.Data;
using Infrastructure.Data.Npgsql;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using SevenStar.Shared.Domain;
using SevenStar.Shared.Domain.Entity;
using SevenStar.Shared.Domain.Repository;
using SevenStar.Shared.Domain.Service;
using System.Data.Common;

namespace SevenStar.ApiService.Controllers;

public class ValuesController : ApiControllerBase
{
    private readonly NpgsqlDataSource _dataSource;
    private readonly ICompanyGameDb _companyGameDb;
    private readonly IUserService _userService;
    

    public ValuesController(NpgsqlDataSource dataSource, ICompanyGameDb companyGameDb)
    {
        _dataSource = dataSource;
        _companyGameDb = companyGameDb;
        // _userService = userService;
    }

    [HttpGet]
    public async Task<string> TestConnect()
    {
        // 每個 Request Scope 只需要 OpenConnectionAsync 一次即可，後續的查詢都可以使用同一個連線
        await using var connection = await _dataSource.OpenConnectionAsync();

        // 使用 Dapper 查詢
        var users = await connection.QueryAsync<UserEntity>("SELECT \"Id\", \"Name\" FROM public.\"User\" /**where**/");
        

        return "abc";
    }

    [HttpGet]
    public async Task<string> TestUnitOfWork()
    {
        var userRepository1 = await _companyGameDb.GetRepository<IUserRepository>();
        var users1 = await userRepository1.GetAsync();

        await _companyGameDb.ExecuteAsync(async (transaction) =>
        {
            var userRepository = await _companyGameDb.GetRepository<IUserRepository>();
            var users = await userRepository.GetAsync();
        });

        // var userRepository2 = await _companyGameDb.GetRepository<IUserRepository>();
        var users2 = await userRepository1.GetAsync();

        return "abc";
    }
}
