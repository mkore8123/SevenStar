using Common.Api.Authen.Jwt.Model;
using SevenStar.Shared.Domain.DbContext.Platform.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace SevenStar.Shared.Domain.Api.Auth.Jwt.Mapper;

/// <summary>
/// JWT 簽章金鑰實體與模型的轉換工具類別。
/// </summary>
public static class JwtSigningKeyMapper
{
    /// <summary>
    /// 將資料庫實體 <see cref="JwtSigningKeyEntity"/> 轉換為應用層模型 <see cref="JwtSigningKey"/>。
    /// </summary>
    /// <param name="entity">簽章金鑰資料表實體</param>
    /// <returns>應用層可使用的簽章金鑰模型</returns>
    public static JwtSigningKey ToModel(JwtSigningKeyEntity entity)
    {
        return new JwtSigningKey
        {
            KeyId = entity.KeyId,
            Algorithm = entity.Algorithm,
            PublicKey = entity.PublicKey,
            PrivateKey = entity.PrivateKey
            // SecurityKey 可由服務層或快取層後續產出，不建議於此轉換階段注入
        };
    }

    /// <summary>
    /// 將應用層模型 <see cref="JwtSigningKey"/> 轉換為資料表實體 <see cref="JwtSigningKeyEntity"/>。
    /// </summary>
    /// <param name="model">應用層簽章金鑰模型</param>
    /// <param name="configId">所屬 JWT 設定的主鍵 ID（config_id）</param>
    /// <returns>資料庫實體模型</returns>
    public static JwtSigningKeyEntity ToEntity(JwtSigningKey model, int configId)
    {
        return new JwtSigningKeyEntity
        {
            ConfigId = configId,
            KeyId = model.KeyId,
            Algorithm = model.Algorithm,
            PublicKey = model.PublicKey,
            PrivateKey = model.PrivateKey,
            ValidFrom = DateTime.UtcNow,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
            // ValidTo 可由外部補上，也可為 null 表示永久有效
        };
    }
}