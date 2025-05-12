using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Infrastructure.Data.Npgsql;


public class NpgsqlUnitOfWork : INpgsqlUnitOfWork 
{
    private readonly IServiceProvider _provider;
    private readonly NpgsqlDataSource _dataSource;

    private readonly Dictionary<Type, object> _repoCache = new();

    public NpgsqlConnection   Cconnection { get; set; }

    public NpgsqlTransaction? Transaction { get; set; } = null;

    public NpgsqlUnitOfWork(IServiceProvider provider, NpgsqlDataSource dataSource)
    {
        _provider = provider;
        _dataSource = dataSource;
    }

    public virtual async Task ExecuteAsyncV2(Func<NpgsqlUnitOfWork, NpgsqlTransaction, Task> operation)
    {
        Cconnection = await _dataSource.OpenConnectionAsync();
        Transaction = await Cconnection.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted);

        try
        {
            await operation(this, Transaction);
            await Transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            // _logger.LogError(ex, "Transaction failed, rolling back.");
            await Transaction.RollbackAsync();
            throw;
        }
        finally
        {
            await Cconnection.CloseAsync();
            await Cconnection.DisposeAsync(); // 建議補上釋放資源
            Cconnection = null;
        }
    }
}
