using System.Text.Json;
using Common.Api.Authen.Jwt.Model;
using SevenStar.Shared.Domain.DbContext.Platform.Entity;

namespace SevenStar.Shared.Domain.Api.Mapper;

public static class JwtTokenConfigMapper
{
    /// <summary>
    /// 將資料庫實體 JwtTokenConfigEntity 轉換為應用層 JwtTokenConfig。
    /// </summary>
    public static JwtTokenConfig ToModel(this JwtTokenConfigEntity entity)
    {
        return new JwtTokenConfig
        {
            Issuer = entity.Issuer,
            Audience = entity.Audience,
            Subject = entity.Subject,
            NotBefore = entity.NotBefore,
            Lifetime = entity.LifetimeMinutes.HasValue ? TimeSpan.FromMinutes(entity.LifetimeMinutes.Value) : null,
            RequireExpirationTime = entity.RequireExp,
            ValidateIssuer = entity.ValidateIssuer,
            ValidateAudience = entity.ValidateAudience,
            ValidateLifetime = entity.ValidateLifetime,
            ClockSkew = entity.ClockSkewSeconds.HasValue ? TimeSpan.FromSeconds(entity.ClockSkewSeconds.Value) : null,
            TokenType = entity.TokenType,
            ValidIssuers = entity.ValidIssuers?.ToList(),
            ValidAudiences = entity.ValidAudiences?.ToList(),
            DefaultClaims = TryDeserialize<Dictionary<string, string>>(entity.DefaultClaims),
            ExtraHeader = TryDeserialize<Dictionary<string, object>>(entity.ExtraHeader),
            ExtraPayload = TryDeserialize<Dictionary<string, object>>(entity.ExtraPayload),
            // 預設 JtiGenerator 為 Guid
            JtiGenerator = () => Guid.NewGuid().ToString(),
            // JwtEnvelopeType 可透過其他欄位判斷；此處預設為 JWS
            // Type = JwtEnvelopeType.JWS
        };
    }

    /// <summary>
    /// 將應用層 JwtTokenConfig 轉換為資料庫實體 JwtTokenConfigEntity。
    /// </summary>
    public static JwtTokenConfigEntity ToEntity(this JwtTokenConfig model, int companyId, int versionNo = 1)
    {
        return new JwtTokenConfigEntity
        {
            CompanyId = companyId,
            Issuer = model.Issuer,
            Audience = model.Audience,
            Subject = model.Subject,
            NotBefore = model.NotBefore,
            LifetimeMinutes = model.Lifetime?.TotalMinutes is double m ? (int?)m : null,
            RequireExp = model.RequireExpirationTime,
            ValidateIssuer = model.ValidateIssuer,
            ValidateAudience = model.ValidateAudience,
            ValidateLifetime = model.ValidateLifetime,
            ClockSkewSeconds = model.ClockSkew.HasValue ? (int)model.ClockSkew.Value.TotalSeconds : null,
            TokenType = model.TokenType,
            ValidIssuers = model.ValidIssuers?.ToArray(),
            ValidAudiences = model.ValidAudiences?.ToArray(),
            DefaultClaims = TrySerialize(model.DefaultClaims),
            ExtraHeader = TrySerialize(model.ExtraHeader),
            ExtraPayload = TrySerialize(model.ExtraPayload),
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            VersionNo = versionNo
        };
    }

    // ====== JSON 序列化 / 反序列化安全處理 ======

    private static string? TrySerialize<T>(T? value)
    {
        if (value == null) return null;
        try
        {
            return JsonSerializer.Serialize(value);
        }
        catch
        {
            return null;
        }
    }

    private static T? TryDeserialize<T>(string? json)
    {
        if (string.IsNullOrWhiteSpace(json)) return default;
        try
        {
            return JsonSerializer.Deserialize<T>(json);
        }
        catch
        {
            return default;
        }
    }
}