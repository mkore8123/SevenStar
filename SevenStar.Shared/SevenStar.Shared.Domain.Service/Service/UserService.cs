using Common.Attributes;
using Common.Enums;
using Microsoft.Extensions.DependencyInjection;
using SevenStar.Shared.Domain.DbContext.Company;
using SevenStar.Shared.Domain.DbContext.Company.Entity;
using SevenStar.Shared.Domain.Enums;
using SevenStar.Shared.Domain.Service;
using System.Data;

namespace SevenStar.Shared.Domain.Imp.Service;

[KeyedService(MemberLevelEnum.Member, ServiceLifetime.Scoped)]
public class UserService : IUserService
{
    private readonly IServiceProvider _provider;

    public UserService(IServiceProvider provider)
    {
        _provider = provider;
    }

    public async Task<Func<IDbTransaction, Task>> PrepareCreateMemberAsync(ICompanyGameDb companyDb, string name)
    {
        var value = await Task.FromResult(1);
        return async (transaction) =>
        {
            await companyDb.User.CreateAsync(new UserEntity { Name = name }, transaction);
            //var repository = _companyDb.GetRepository<IUserRepository>();
            //var result = await repository.CreateAsync(new UserEntity { Name = name }, transaction);

            //return;
        };
    }
}
