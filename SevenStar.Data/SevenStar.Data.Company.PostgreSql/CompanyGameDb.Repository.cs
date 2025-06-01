using Microsoft.Extensions.DependencyInjection;
using SevenStar.Data.Company.PostgreSql.Repository.Company;
using SevenStar.Shared.Domain.DbContext.Company.Repository;
using SevenStar.Shared.Domain.Enums;
using SevenStar.Shared.Domain.Service;

namespace SevenStar.Data.Company.PostgreSql;

public partial class CompanyGameDb
{
    private IUserRepository? _user;

    public IUserRepository User => _user ??= new UserRepository(Connection);
}