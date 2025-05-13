using Npgsql;
using Infrastructure.Data.Npgsql;
using SevenStar.Shared.Domain;
using SevenStar.Shared.Domain.Repository;
using SevenStar.Data.Company.Npgsql.Repository;

namespace SevenStar.Data.Company.Nppgsql;

public partial class CompanyGameDb : NpgsqlUnitOfWork, ICompanyGameDb
{
    private readonly Dictionary<Type, object> _repoCache = new();

    public CompanyGameDb(IServiceProvider provider, NpgsqlDataSource dataSource) : base(dataSource)
    {
    }

    public async Task<TRepository> GetRepository<TRepository>() where TRepository : class
    {
        await OpenConnectionAsync();
        var type = typeof(TRepository);

        if (_repoCache.TryGetValue(type, out var repositoryInstance))
        {
            var npgRepositoryInstance = (INpgsqlRepository<TRepository>)repositoryInstance;
            npgRepositoryInstance.Transaction = Transaction;

            return (TRepository)repositoryInstance;
        }
            
        // 建立對應的實例（你可依介面判斷對應哪個實作）
        object repository = type switch
        {
            var t when t == typeof(IUserRepository) => new UserRepository(Cconnection, Transaction),
            _ => throw new NotSupportedException($"Repository type not registered: {type.FullName}")
        };

        _repoCache[type] = repository;
        return (TRepository)repository;
    }
}
