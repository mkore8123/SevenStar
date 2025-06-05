using Common.Api.Auth.Jwt;
using Common.Api.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SevenStar.Shared.Domain.Api.Authen.Claim;
using SevenStar.Shared.Domain.Api.Authen.Jwt;
using SevenStar.Shared.Domain.DbContext.Platform.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace SevenStar.Shared.Domain.Api.Auth.Jwt;

/// <summary>
/// 提供 JwtTokenConfigEntity 與 JwtSigningKeyEntity 轉換為 JwtTokenConfig 的擴充方法。
/// </summary>
public static class JwtTokenConfigEntityExtensions
{
    public static IServiceCollection AddJwtTokenService(this IServiceCollection services)
    {
        services.TryAddSingleton<IApiSingletonCacheService, ApiSingletonCacheService>();
        services.AddJwtTokenService<MemberClaimModel, MemberClaimMapper, DbJwtTokenConfigProvider, DbJwtSigningKeyProvider>();

        return services;
    }

    /// <summary>
    /// 將 JWT 設定資料表實體 <see cref="JwtTokenConfigEntity"/> 與 JWT 金鑰資料表實體 <see cref="JwtSigningKeyEntity"/>
    /// 轉換為應用程式用的 <see cref="JwtTokenConfig"/> 設定物件。
    /// 適用於產生 Token 或驗證 Token 流程，需要同時取得簽章金鑰與 JWT 配置時。
    /// </summary>
    /// <param name="entity">
    /// JWT 設定資料表實體，包含 JWT 設定主資料（如發行者、接收者、有效時間等）。
    /// </param>
    /// <param name="keyEntity">
    /// JWT 金鑰資料表實體，包含金鑰的 kid、簽章演算法、金鑰內容與啟用期間等資訊。
    /// 通常每組 JWT 設定（entity）可對應一組或多組金鑰（keyEntity）。
    /// </param>
    /// <returns>
    /// 轉換後的 <see cref="JwtTokenConfig"/> 設定物件，結合 JWT 設定與金鑰資料，可用於應用程式產生/驗證 Token。
    /// </returns>
    public static JwtTokenConfig ToModel(this JwtTokenConfigEntity entity, JwtSigningKeyEntity keyEntity)
    {
        return new JwtTokenConfig
        {
            // 基本設定對應
            Issuer = entity.Issuer,
            Audience = entity.Audience,
            // 金鑰設定由 keyEntity 提供
            KeyId = keyEntity.KeyId,
            Algorithm = keyEntity.Algorithm,
            // 時效、驗證邏輯欄位
            Lifetime = entity.LifetimeMinutes.HasValue ? TimeSpan.FromMinutes(entity.LifetimeMinutes.Value) : null,
            RequireExpirationTime = entity.RequireExp,
            ValidateIssuer = entity.ValidateIssuer,
            ValidateAudience = entity.ValidateAudience,
            ValidateLifetime = entity.ValidateLifetime,
            ClockSkew = entity.ClockSkewSeconds.HasValue ? TimeSpan.FromSeconds(entity.ClockSkewSeconds.Value) : null,
            // 多組 Issuer/Audience 支援
            ValidIssuers = entity.ValidIssuers != null ? new List<string>(entity.ValidIssuers) : null,
            ValidAudiences = entity.ValidAudiences != null ? new List<string>(entity.ValidAudiences) : null,
            // 其他欄位對應
            Subject = entity.Subject,
            NotBefore = entity.NotBefore,
            TokenType = entity.TokenType,
            // JSON 欄位反序列化為字典型態
            DefaultClaims = ParseJsonDictionary(entity.DefaultClaims),
            ExtraHeader = ParseJsonObject(entity.ExtraHeader),
            ExtraPayload = ParseJsonObject(entity.ExtraPayload),
            // JtiGenerator、EncryptingCredentials 若有特殊需求可於此擴充
        };
    }

    /// <summary>
    /// 將 JSON 字串（Key/Value 格式）反序列化為 Dictionary&lt;string, string&gt;。
    /// 若輸入為 null 或空字串，則回傳 null。
    /// </summary>
    /// <param name="json">來源 JSON 字串，格式應為 { "key": "value" }</param>
    /// <returns>Dictionary 物件，或 null。</returns>
    private static Dictionary<string, string>? ParseJsonDictionary(string? json)
    {
        if (string.IsNullOrWhiteSpace(json)) return null;
        return JsonSerializer.Deserialize<Dictionary<string, string>>(json);
    }

    /// <summary>
    /// 將 JSON 字串（Key/Object 格式）反序列化為 Dictionary&lt;string, object&gt;。
    /// 用於還原 ExtraHeader、ExtraPayload 等欄位。
    /// </summary>
    /// <param name="json">來源 JSON 字串，格式應為 { "key": "object value" }</param>
    /// <returns>Dictionary 物件，或 null。</returns>
    private static Dictionary<string, object>? ParseJsonObject(string? json)
    {
        if (string.IsNullOrWhiteSpace(json)) return null;
        return JsonSerializer.Deserialize<Dictionary<string, object>>(json);
    }
}