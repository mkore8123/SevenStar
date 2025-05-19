
using Common.Api.Token.Jwt;
using Dapper;
using EasyCaching.Core;
using EasyCaching.Core.Serialization;
using Infrastructure.Caching.Redis;
using Infrastructure.Caching.Redis.Implement;
using Infrastructure.Data;
using Infrastructure.Data.Npgsql;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using SevenStar.Shared.Domain;
using SevenStar.Shared.Domain.Api.Token;
using SevenStar.Shared.Domain.Entity.Company;
using SevenStar.Shared.Domain.Repository;
using SevenStar.Shared.Domain.Service;
using StackExchange.Redis;
using System.Data.Common;
using System.Diagnostics;

namespace SevenStar.ApiService.Controllers;

public class ValuesController : ApiControllerBase
{
    private readonly ICompanyGameDb _companyGameDb;
    private readonly JwtTokenService _tokenService;
    private readonly IUserService _userService;
    private readonly IHybridCachingProvider _cache;

    public ValuesController(IServiceProvider provider, ICompanyGameDb companyGameDb, JwtTokenService tokenService /*, IHybridProviderFactory factory*/)
    {
        _companyGameDb = companyGameDb;
        _tokenService = tokenService;
    }

    [HttpGet]
    public async Task<string> TestUnitOfWork()
    {
        var model = new UserClaimModel()
        {
            UserId = 1,
            UserName = "test123"
        };

        var jwt = _tokenService.GenerateToken(model);
        return await Task.FromResult(jwt);
    }
}
