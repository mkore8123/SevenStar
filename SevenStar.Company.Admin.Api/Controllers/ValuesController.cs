using Microsoft.AspNetCore.Mvc;
using SevenStar.Shared.Domain.Api.Token;
using SevenStar.Shared.Domain.Api.Token.Jwt;
using SevenStar.Shared.Domain.Service;

namespace SevenStar.ApiService.Controllers;

public class ValuesController : ApiControllerBase
{
    private readonly Shared.Domain.Database.ICompanyGameDb _companyGameDb;
    private readonly JwtTokenManager _tokenService;
    private readonly IUserService _userService;

    public ValuesController(IServiceProvider provider, Shared.Domain.Database.ICompanyGameDb companyGameDb, JwtTokenManager tokenService /*, IHybridProviderFactory factory*/)
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
