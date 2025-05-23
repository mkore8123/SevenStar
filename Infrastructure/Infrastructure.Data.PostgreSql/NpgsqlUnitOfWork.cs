using Npgsql;
using System.Data;
using Infrastructure.Data.Npgsql.Interface;

namespace Infrastructure.Data.Npgsql;


public class NpgsqlUnitOfWork : INpgsqlUnitOfWork
{
    public NpgsqlConnection Connection { get; set; }

    public NpgsqlUnitOfWork(NpgsqlConnection connection)
    {
        Connection = connection;
    }

    public virtual async Task ExecuteAsync(Func<IDbTransaction, Task> operation, IsolationLevel transLevel = IsolationLevel.ReadCommitted)
    {
        await OpenConnectionAsync();
        await using var transaction = await Connection.BeginTransactionAsync(transLevel);

        try
        {
            await operation(transaction);
            await transaction.CommitAsync();
        }
        catch (NpgsqlException ex)
        {
            await transaction.RollbackAsync();
            throw;
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
        if (Connection.State != ConnectionState.Open)
            await Connection.OpenAsync();
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
