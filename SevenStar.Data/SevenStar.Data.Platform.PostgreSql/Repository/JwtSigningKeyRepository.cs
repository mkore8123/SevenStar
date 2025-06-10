using Dapper;
using Npgsql;
using SevenStar.Shared.Domain.DbContext.Platform.Entity;
using SevenStar.Shared.Domain.DbContext.Platform.Repository;

namespace SevenStar.Data.Platform.PostgreSql.Repository;

/// <summary>
/// JWT 金鑰管理 Repository 非同步實作
/// </summary>
public class JwtSigningKeyRepository : IJwtSigningKeyRepository
{
    private readonly NpgsqlConnection _connection;

    /// <summary>
    /// 建構子，需提供 PostgreSQL NpgsqlConnection
    /// </summary>
    /// <param name="connection">已開啟的 NpgsqlConnection</param>
    public JwtSigningKeyRepository(NpgsqlConnection connection)
    {
        _connection = connection;
    }

    /// <inheritdoc/>
    public async Task<int> CreateAsync(JwtSigningKeyEntity key)
    {
        var sql = @"
            INSERT INTO jwt_signing_key
            (config_id, key_id, algorithm, public_key, private_key, valid_from, valid_to, is_active)
            VALUES
            (@ConfigId, @KeyId, @Algorithm, @PublicKey, @PrivateKey, @ValidFrom, @ValidTo, @IsActive)
            RETURNING id";
        return await _connection.ExecuteScalarAsync<int>(sql, key);
    }

    /// <inheritdoc/>
    public async Task<JwtSigningKeyEntity?> GetByIdAsync(int id)
    {
        var sql = @"
            SELECT id, config_id, key_id, algorithm, public_key, private_key, 
                   valid_from, valid_to, is_active, created_at
            FROM jwt_signing_key
            WHERE id = @Id";
        return await _connection.QueryFirstOrDefaultAsync<JwtSigningKeyEntity>(sql, new { Id = id });
    }

    /// <inheritdoc/>
    public async Task<List<JwtSigningKeyEntity>> GetByConfigIdAsync(int configId)
    {
        var sql = @"
            SELECT id, config_id, key_id, algorithm, public_key, private_key, 
                   valid_from, valid_to, is_active, created_at
            FROM jwt_signing_key
            WHERE config_id = @ConfigId";
        
        var result = await _connection.QueryAsync<JwtSigningKeyEntity>(sql, new { ConfigId = configId });

        return result.ToList();
    }

    /// <inheritdoc/>
    public async Task<List<JwtSigningKeyEntity>> GetByKeyIdAsync(string keyId)
    {
        var sql = @"
            SELECT id, config_id, key_id, algorithm, public_key, private_key, 
                   valid_from, valid_to, is_active, created_at
            FROM jwt_signing_key
            WHERE key_id = @KeyId";

        var result = await _connection.QueryAsync<JwtSigningKeyEntity>(sql, new { KeyId = keyId });

        return result.ToList();
    }

    /// <inheritdoc/>
    public async Task UpdateAsync(JwtSigningKeyEntity key)
    {
        var sql = @"
            UPDATE jwt_signing_key SET
                config_id = @ConfigId,
                key_id = @KeyId,
                algorithm = @Algorithm,
                public_key = @PublicKey,
                private_key = @PrivateKey,
                valid_from = @ValidFrom,
                valid_to = @ValidTo,
                is_active = @IsActive
            WHERE id = @Id";
        await _connection.ExecuteAsync(sql, key);
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(int id)
    {
        var sql = @"DELETE FROM jwt_signing_key WHERE id = @Id";
        await _connection.ExecuteAsync(sql, new { Id = id });
    }
}
