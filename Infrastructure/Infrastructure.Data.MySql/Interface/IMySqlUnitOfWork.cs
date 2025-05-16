using MySqlConnector;
using Infrastructure.Data.Interface;

namespace Infrastructure.Data.MySql.Interface;

public interface IMySqlUnitOfWork : IUnitOfWork
{
    MySqlConnection Connection { get; set; }
}