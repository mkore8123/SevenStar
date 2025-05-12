using Common.Enum;
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

    public async Task Create(string name)
    {
        var user = new UserEntity { Name = name };

        await _companyDb.ExecuteAsyncV2(async (uow, transaction) =>
        {
            await _companyDb.GetRepository<IUserRepository>().Set(transaction).GetUsersAsync();

        });

        // _unitOfWork.GetRepository<IUserRepository>();
        //await _unitOfWork.ExecuteAsync(async (connection, transaction) =>
        //{

        //    await _userRepository.Set(connection, transaction).Create(user);
        //});
    }
}
