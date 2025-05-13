using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using System.Transactions;

namespace Infrastructure.Data.Npgsql;


public class NpgsqlUnitOfWork : INpgsqlUnitOfWork
{
    protected readonly NpgsqlDataSource _dataSource;

    public NpgsqlConnection Cconnection { get; set; } //= null;

    public NpgsqlTransaction? Transaction { get; set; } = null;

    public NpgsqlUnitOfWork(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    protected async Task OpenConnectionAsync()
    {
        Cconnection ??= await _dataSource.OpenConnectionAsync();
    }

    public virtual async Task ExecuteAsync(Func<NpgsqlTransaction, Task> operation)
    {
        await OpenConnectionAsync();
        Transaction = await Cconnection.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted);


        try
        {
            await operation(Transaction);
            await Transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await Transaction.RollbackAsync();
            throw;
        }
        finally
        {
            await Transaction.DisposeAsync();
            Transaction = null;
        }
    }

    public async ValueTask DisposeAsync()
    {
        if(Transaction != null)
            await Transaction.DisposeAsync();

        if (Cconnection != null)
        {
            await Cconnection.CloseAsync();
            await Cconnection.DisposeAsync();
        }
    }

    public void Dispose()
    {
        Transaction?.Dispose();

        if (Cconnection != null)
        {
            Cconnection.Close();
            Cconnection.Dispose();
        }
    }
}
