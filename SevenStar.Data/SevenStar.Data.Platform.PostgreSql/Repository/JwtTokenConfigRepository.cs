using Dapper;
using Npgsql;
using SevenStar.Shared.Domain.DbContext.Platform.Entity;
using SevenStar.Shared.Domain.DbContext.Platform.Repository;

namespace SevenStar.Data.Platform.PostgreSql.Repository;

/// <summary>
/// JWT 設定 Repository 非同步實作
/// </summary>
public class JwtTokenConfigRepository : IJwtTokenConfigRepository
{
    private readonly NpgsqlConnection _connection;

    /// <summary>
    /// 建構子，需提供 PostgreSQL NpgsqlConnection
    /// </summary>
    /// <param name="connection">已開啟的 NpgsqlConnection</param>
    public JwtTokenConfigRepository(NpgsqlConnection connection)
    {
        _connection = connection;
    }

    /// <inheritdoc/>
    public async Task<int> CreateAsync(JwtTokenConfigEntity config)
    {
        var sql = @"
            INSERT INTO jwt_token_config
            (company_id, issuer, audience, lifetime_minutes, require_exp, validate_issuer, validate_audience, validate_lifetime, clock_skew_seconds, subject, not_before, token_type,
             default_claims, extra_header, extra_payload, valid_issuers, valid_audiences, is_active, version_no)
            VALUES
            (@CompanyId, @Issuer, @Audience, @LifetimeMinutes, @RequireExp, @ValidateIssuer, @ValidateAudience, @ValidateLifetime, @ClockSkewSeconds, @Subject, @NotBefore, @TokenType,
             @DefaultClaims::jsonb, @ExtraHeader::jsonb, @ExtraPayload::jsonb, @ValidIssuers, @ValidAudiences, @IsActive, @VersionNo)
            RETURNING id";
        return await _connection.ExecuteScalarAsync<int>(sql, config);
    }

    /// <inheritdoc/>
    public async Task<JwtTokenConfigEntity?> GetByIdAsync(int id)
    {
        var sql = @"
            SELECT id, company_id, issuer, audience, lifetime_minutes, require_exp, validate_issuer, validate_audience, validate_lifetime,
                   clock_skew_seconds, subject, not_before, token_type,
                   default_claims, extra_header, extra_payload,
                   valid_issuers, valid_audiences,
                   is_active, version_no, created_at, updated_at
            FROM jwt_token_config
            WHERE id = @Id";
        return await _connection.QueryFirstOrDefaultAsync<JwtTokenConfigEntity>(sql, new { Id = id });
    }

    /// <inheritdoc/>
    public async Task<List<JwtTokenConfigEntity>> GetByCompanyIdAsync(int companyId)
    {
        var sql = @"
            SELECT id, company_id, issuer, audience, lifetime_minutes, require_exp, validate_issuer, validate_audience, validate_lifetime,
                   clock_skew_seconds, subject, not_before, token_type,
                   default_claims, extra_header, extra_payload,
                   valid_issuers, valid_audiences,
                   is_active, version_no, created_at, updated_at
            FROM jwt_token_config
            WHERE company_id = @CompanyId";
        
        var result = await _connection.QueryAsync<JwtTokenConfigEntity>(sql, new { CompanyId = companyId });
        return result.ToList();
    }

    /// <inheritdoc/>
    public async Task<List<JwtTokenConfigEntity>> GetByIssuerAudienceAsync(string issuer, string audience)
    {
        var sql = @"
            SELECT id, company_id, issuer, audience, lifetime_minutes, require_exp, validate_issuer, validate_audience, validate_lifetime,
                   clock_skew_seconds, subject, not_before, token_type,
                   default_claims, extra_header, extra_payload,
                   valid_issuers, valid_audiences,
                   is_active, version_no, created_at, updated_at
            FROM jwt_token_config
            WHERE issuer = @Issuer AND audience = @Audience";
        
        var result = await _connection.QueryAsync<JwtTokenConfigEntity>(sql, new { Issuer = issuer, Audience = audience });
        return result.ToList();
    }

    /// <inheritdoc/>
    public async Task UpdateAsync(JwtTokenConfigEntity config)
    {
        var sql = @"
            UPDATE jwt_token_config SET
                company_id = @CompanyId,
                issuer = @Issuer,
                audience = @Audience,
                lifetime_minutes = @LifetimeMinutes,
                require_exp = @RequireExp,
                validate_issuer = @ValidateIssuer,
                validate_audience = @ValidateAudience,
                validate_lifetime = @ValidateLifetime,
                clock_skew_seconds = @ClockSkewSeconds,
                subject = @Subject,
                not_before = @NotBefore,
                token_type = @TokenType,
                default_claims = @DefaultClaims::jsonb,
                extra_header = @ExtraHeader::jsonb,
                extra_payload = @ExtraPayload::jsonb,
                valid_issuers = @ValidIssuers,
                valid_audiences = @ValidAudiences,
                is_active = @IsActive,
                version_no = @VersionNo,
                updated_at = now()
            WHERE id = @Id";
        await _connection.ExecuteAsync(sql, config);
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(int id)
    {
        var sql = @"DELETE FROM jwt_token_config WHERE id = @Id";
        await _connection.ExecuteAsync(sql, new { Id = id });
    }

    /// <inheritdoc/>
    public async Task<List<JwtTokenConfigEntity>> GetAllActiveAsync()
    {
        var sql = @"
        SELECT id, company_id, issuer, audience, lifetime_minutes, require_exp, validate_issuer, validate_audience, validate_lifetime,
               clock_skew_seconds, subject, not_before, token_type,
               default_claims, extra_header, extra_payload,
               valid_issuers, valid_audiences,
               is_active, version_no, created_at, updated_at
        FROM jwt_token_config
        WHERE is_active = true
          AND (valid_from IS NULL OR valid_from <= now())
          AND (valid_to IS NULL OR valid_to > now())";

        var result = await _connection.QueryAsync<JwtTokenConfigEntity>(sql);
        return result.ToList();
    }
}

