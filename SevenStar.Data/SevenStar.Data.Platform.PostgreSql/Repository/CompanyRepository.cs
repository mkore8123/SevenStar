using Dapper;
using Npgsql;
using SevenStar.Shared.Domain.DbContext.Platform.Entity;
using SevenStar.Shared.Domain.DbContext.Platform.Repository;

namespace SevenStar.Data.Platform.PostgreSql.Repository;

/// <summary>
/// 公司主檔 Repository 實作
/// </summary>
public class CompanyRepository : ICompanyRepository
{
    private readonly NpgsqlConnection _connection;

    /// <summary>
    /// 建構子，需提供 PostgreSQL NpgsqlConnection
    /// </summary>
    /// <param name="connection">已開啟的 NpgsqlConnection</param>
    public CompanyRepository(NpgsqlConnection connection)
    {
        _connection = connection;
    }

    /// <inheritdoc/>
    public int Create(CompanyEntity company)
    {
        var sql = @"INSERT INTO company (code, name, is_active) 
                    VALUES (@Code, @Name, @IsActive) RETURNING id";
        return _connection.ExecuteScalar<int>(sql, company);
    }

    /// <inheritdoc/>
    public CompanyEntity? GetById(int id)
    {
        var sql = @"
            SELECT id, code, name, is_active, created_at
            FROM company
            WHERE id = @Id";
        return _connection.QueryFirstOrDefault<CompanyEntity>(sql, new { Id = id });
    }

    /// <inheritdoc/>
    public CompanyEntity? GetByCode(string code)
    {
        var sql = @"
            SELECT id, code, name, is_active, created_at
            FROM company
            WHERE code = @Code";
        return _connection.QueryFirstOrDefault<CompanyEntity>(sql, new { Code = code });
    }

    /// <inheritdoc/>
    public IEnumerable<CompanyEntity> GetAll()
    {
        var sql = @"
            SELECT id, code, name, is_active, created_at
            FROM company";
        return _connection.Query<CompanyEntity>(sql);
    }

    /// <inheritdoc/>
    public void Update(CompanyEntity company)
    {
        var sql = @"UPDATE company 
                    SET code = @Code, name = @Name, is_active = @IsActive 
                    WHERE id = @Id";
        _connection.Execute(sql, company);
    }

    /// <inheritdoc/>
    public void Delete(int id)
    {
        var sql = @"DELETE FROM company WHERE id = @Id";
        _connection.Execute(sql, new { Id = id });
    }
}