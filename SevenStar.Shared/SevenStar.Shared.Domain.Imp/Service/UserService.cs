using Common.Enums;
using Infrastructure.Data.Npgsql;
using Microsoft.Extensions.DependencyInjection;
using SevenStar.Shared.Domain.Entity;
using SevenStar.Shared.Domain.Repository;
using SevenStar.Shared.Domain.Service;


namespace SevenStar.Shared.Domain.Imp.Service;


public class UserService : IUserService
{
    private readonly ICompanyGameDb _companyDb;

    public UserService(IServiceProvider serviceProvider)
    {
        _companyDb = serviceProvider.GetRequiredService<ICompanyGameDb>();
    }

    public async Task CreateAsync(string name)
    {
        var user = new UserEntity { Name = name };

        await _companyDb.ExecuteAsync(async (transaction) =>
        {
            
            
        });
    }
}
