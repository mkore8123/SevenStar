using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using System.Data;
using System.Transactions;

namespace Infrastructure.Data.Npgsql;


public class NpgsqlUnitOfWork : INpgsqlUnitOfWork
{
    protected readonly NpgsqlDataSource _dataSource;

    public NpgsqlConnection Connection { get; set; }

    public NpgsqlUnitOfWork(NpgsqlConnection connection)
    {
        Connection = connection;
    }

    public NpgsqlUnitOfWork(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public virtual async Task ExecuteAsync(Func<IDbTransaction, Task> operation)
    {
        await OpenConnectionAsync();
        await using var transaction = await Connection.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted);

        try
        {
            await operation(transaction);
            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw;
        }
        finally
        {
            await transaction.DisposeAsync();
        }
    }

    protected async Task OpenConnectionAsync()
    {
        if (Connection == null)
        {
            Connection = await _dataSource.OpenConnectionAsync();
        }
        else if (Connection.State != ConnectionState.Open)
        {
            Connection.Open();
        }
    }


    public async ValueTask DisposeAsync()
    {
        if (Connection != null)
        {
            await Connection.CloseAsync();
            await Connection.DisposeAsync();
        }
    }

    public void Dispose()
    {
        if (Connection != null)
        {
            Connection.Close();
            Connection.Dispose();
        }
    }
}
