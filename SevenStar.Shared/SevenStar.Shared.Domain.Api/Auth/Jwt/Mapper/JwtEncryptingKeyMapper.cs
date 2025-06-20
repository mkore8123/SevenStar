using Common.Api.Authen.Jwt.Model;
using SevenStar.Shared.Domain.DbContext.Platform.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace SevenStar.Shared.Domain.Api.Auth.Jwt.Mapper;

/// <summary>
/// JWT 加密金鑰實體與模型的轉換工具類別。
/// </summary>
public static class JwtEncryptingKeyMapper
{
    /// <summary>
    /// 將資料表實體 <see cref="JwtEncryptingKeyEntity"/> 轉換為應用層使用的 <see cref="JwtEncryptingKey"/>。
    /// </summary>
    /// <param name="entity">資料庫實體</param>
    /// <returns>應用層 JWE 金鑰模型</returns>
    public static JwtEncryptingKey ToModel(JwtEncryptingKeyEntity entity)
    {
        return new JwtEncryptingKey
        {
            KeyId = entity.KeyId,
            Algorithm = entity.Algorithm,
            ContentAlgorithm = entity.ContentAlg,
            PublicKey = entity.PublicKey,
            PrivateKey = entity.PrivateKey
        };
    }

    /// <summary>
    /// 將應用層使用的 <see cref="JwtEncryptingKey"/> 轉換為資料表用的 <see cref="JwtEncryptingKeyEntity"/>。
    /// </summary>
    /// <param name="model">應用層金鑰模型</param>
    /// <param name="configId">對應 JWT 設定的 ConfigId，需由外部提供</param>
    /// <returns>資料庫金鑰實體</returns>
    public static JwtEncryptingKeyEntity ToEntity(JwtEncryptingKey model, int configId)
    {
        return new JwtEncryptingKeyEntity
        {
            ConfigId = configId,
            KeyId = model.KeyId,
            Algorithm = model.Algorithm,
            ContentAlg = model.ContentAlgorithm,
            PublicKey = model.PublicKey,
            PrivateKey = model.PrivateKey,
            // 以下欄位視情況補上預設值
            ValidFrom = DateTime.UtcNow,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }
}