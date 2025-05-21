using Microsoft.Extensions.DependencyInjection;
using SevenStar.Shared.Domain.Database;
using SevenStar.Shared.Domain.Entity.Company;
using SevenStar.Shared.Domain.Repository;
using SevenStar.Shared.Domain.Service;
using System.Data;


namespace SevenStar.Shared.Domain.Imp.Service;


public class UserService : IUserService
{
    private readonly Database.ICompanyGameDb _companyDb;

    public UserService(IServiceProvider serviceProvider)
    {
        _companyDb = serviceProvider.GetRequiredService<Database.ICompanyGameDb>();
    }

    public async Task<Func<IDbTransaction, Task>> PrepareCreateAsync(string name)
    {
        var value = await Task.FromResult(1);

        return async (transaction) =>
        {
            var repository = _companyDb.GetRepository<IUserRepository>();
            var result = await repository.CreateAsync(new UserEntity { Name = name }, transaction);

            return;
        };
    }
}
