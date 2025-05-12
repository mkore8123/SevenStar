
using Dapper;
using Infrastructure.Data.Npgsql;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using SevenStar.Shared.Domain.Entity;
using System.Data.Common;

namespace SevenStar.ApiService.Controllers;

public class ValuesController : ApiControllerBase
{
    private readonly NpgsqlDataSource _dataSource;
    private readonly IUnitOfWork _unitOfWork;

    public ValuesController(NpgsqlDataSource dataSource, INpgsqlUnitOfWork unitOfWork)
    {
        _dataSource = dataSource;
        _unitOfWork = unitOfWork;
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
        _unitOfWork

        return "abc";
    }
}
