
using Dapper;
using Infrastructure.Data;
using Infrastructure.Data.Npgsql;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using SevenStar.Shared.Domain;
using SevenStar.Shared.Domain.Entity.Company;
using SevenStar.Shared.Domain.Repository;
using SevenStar.Shared.Domain.Service;
using System.Data.Common;

namespace SevenStar.ApiService.Controllers;

public class ValuesController : ApiControllerBase
{
    private readonly ICompanyGameDb _companyGameDb;
    private readonly IUserService _userService;
    

    public ValuesController(ICompanyGameDb companyGameDb)
    {
        _companyGameDb = companyGameDb;
    }

    [HttpGet]
    public async Task<string> TestUnitOfWork()
    {
        // var userRepository = _companyGameDb.GetRepository<IUserRepository>();
        // var user = await userRepository.GetAsync(1);

        await _companyGameDb.ExecuteAsync(async (transaction) =>
        {
            //await userRepository.Create(user, transaction);
            //await userRepository.Create(user, transaction);
        });


        return "abc";
    }
}
