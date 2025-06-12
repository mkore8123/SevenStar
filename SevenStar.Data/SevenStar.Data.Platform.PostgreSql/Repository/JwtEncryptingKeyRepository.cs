using Dapper;
using Npgsql;
using SevenStar.Shared.Domain.DbContext.Platform.Entity;
using SevenStar.Shared.Domain.DbContext.Platform.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace SevenStar.Data.Platform.PostgreSql.Repository;

/// <summary>
/// JWT 加密金鑰 Repository（PostgreSQL）非同步實作。
/// </summary>
public class JwtEncryptingKeyRepository : IJwtEncryptingKeyRepository
{
    private readonly NpgsqlConnection _connection;

    /// <summary>
    /// 建構子，需傳入已開啟的 NpgsqlConnection。
    /// </summary>
    /// <param name="connection">PostgreSQL 資料庫連線。</param>
    public JwtEncryptingKeyRepository(NpgsqlConnection connection)
    {
        _connection = connection;
    }

    /// <inheritdoc/>
    public async Task<int> CreateAsync(JwtEncryptingKeyEntity key)
    {
        var sql = @"
            INSERT INTO jwt_encrypting_key
            (config_id, key_id, algorithm, content_alg, public_key, private_key, valid_from, valid_to, is_active)
            VALUES
            (@ConfigId, @KeyId, @Algorithm, @ContentAlg, @PublicKey, @PrivateKey, @ValidFrom, @ValidTo, @IsActive)
            RETURNING id";
        return await _connection.ExecuteScalarAsync<int>(sql, key);
    }

    /// <inheritdoc/>
    public async Task<JwtEncryptingKeyEntity?> GetByIdAsync(int id)
    {
        var sql = @"
            SELECT id, config_id, key_id, algorithm, content_alg, public_key, private_key,
                   valid_from, valid_to, is_active, created_at
            FROM jwt_encrypting_key
            WHERE id = @Id";
        return await _connection.QueryFirstOrDefaultAsync<JwtEncryptingKeyEntity>(sql, new { Id = id });
    }

    /// <inheritdoc/>
    public async Task<List<JwtEncryptingKeyEntity>> GetByConfigIdAsync(int configId)
    {
        var sql = @"
            SELECT id, config_id, key_id, algorithm, content_alg, public_key, private_key,
                   valid_from, valid_to, is_active, created_at
            FROM jwt_encrypting_key
            WHERE config_id = @ConfigId";
        var result = await _connection.QueryAsync<JwtEncryptingKeyEntity>(sql, new { ConfigId = configId });
        return result.ToList();
    }

    /// <inheritdoc/>
    public async Task<List<JwtEncryptingKeyEntity>> GetByKeyIdAsync(string keyId)
    {
        var sql = @"
            SELECT id, config_id, key_id, algorithm, content_alg, public_key, private_key,
                   valid_from, valid_to, is_active, created_at
            FROM jwt_encrypting_key
            WHERE key_id = @KeyId";
        var result = await _connection.QueryAsync<JwtEncryptingKeyEntity>(sql, new { KeyId = keyId });
        return result.ToList();
    }

    /// <inheritdoc/>
    public async Task UpdateAsync(JwtEncryptingKeyEntity key)
    {
        var sql = @"
            UPDATE jwt_encrypting_key SET
                config_id = @ConfigId,
                key_id = @KeyId,
                algorithm = @Algorithm,
                content_alg = @ContentAlg,
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
        var sql = @"DELETE FROM jwt_encrypting_key WHERE id = @Id";
        await _connection.ExecuteAsync(sql, new { Id = id });
    }

    /// <inheritdoc/>
    public async Task<List<JwtEncryptingKeyEntity>?> GetAllActiveAsync()
    {
        var sql = @"
            SELECT id, config_id, key_id, algorithm, content_alg, public_key, private_key,
                   valid_from, valid_to, is_active, created_at
            FROM jwt_encrypting_key
            WHERE is_active = true
              AND valid_from <= now()
              AND (valid_to IS NULL OR valid_to > now())";

        var result = await _connection.QueryAsync<JwtEncryptingKeyEntity>(sql);
        return result.ToList();
    }
}