﻿using Microsoft.AspNetCore.Mvc;
using SevenStar.Shared.Domain.Api.Auth;
using SevenStar.Shared.Domain.Api.Auth.Jwt;
using SevenStar.Shared.Domain.DbContext.Company;
using SevenStar.Shared.Domain.DbContext.Platform;
using SevenStar.Shared.Domain.Enums;
using SevenStar.Shared.Domain.Service;

namespace SevenStar.ApiService.Controllers;

public class ValuesController : ApiControllerBase
{
    private readonly IServiceProvider _provider;
    private readonly ICompanyGameDb _companyDb;

    public ValuesController(IServiceProvider provider, ICompanyGameDb companyDb)
    {
        //_db = db;
        //_dbFactory = dbFactory;
        _provider = provider;
        _companyDb = companyDb;
    }

    [HttpGet]
    public async Task<string> Test()
    {
        // var platformDb = _dbFactory.CreatePlatformDbAsync();
        var abc = _provider.GetRequiredKeyedService<IUserService>(MemberLevelEnum.Member);
        var fun = await abc.PrepareCreateMemberAsync(_companyDb, "111");
        // await _companyDb.UserService.PrepareCreateMemberAsync("test");
        var user1 = _companyDb.User.GetAsync();
        var user2 = _companyDb.User.GetAsync();
        return "abc";
    }
}
