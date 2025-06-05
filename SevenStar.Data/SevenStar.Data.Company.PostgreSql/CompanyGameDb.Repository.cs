using SevenStar.Data.Company.PostgreSql.Repository;
using SevenStar.Shared.Domain.DbContext.Company.Repository;

namespace SevenStar.Data.Company.PostgreSql;

public partial class CompanyGameDb
{
    private IUserRepository? _user;

    public IUserRepository User => _user ??= new UserRepository(Connection);
}