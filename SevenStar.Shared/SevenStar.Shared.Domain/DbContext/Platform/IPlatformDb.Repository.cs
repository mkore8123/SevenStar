using System;
using System.Collections.Generic;
using System.Text;
using SevenStar.Shared.Domain.DbContext.Platform.Repository;

namespace SevenStar.Shared.Domain.DbContext.Platform;

public partial interface IPlatformDb
{
    /// <summary>
    /// 公司主檔 Repository 介面，提供公司資料的 CRUD 操作
    /// </summary>
    ICompanyRepository Company { get; }

    /// <summary>
    /// JWT 金鑰管理 Repository 介面，提供金鑰 CRUD 操作
    /// </summary>
    IJwtSigningKeyRepository JwtSigningKey { get; }

    /// <summary>
    /// JWT 設定 Repository 介面，提供 JWT Token 配置資料的 CRUD 操作
    /// </summary>
    IJwtTokenConfigRepository JwtTokenConfig { get; }

    /// <summary>
    /// JWT 加密金鑰管理 Repository 介面，提供 JWT 加密金鑰（JWE）的非同步 CRUD 與查詢操作，
    /// 包含金鑰新增、查詢、輪替、效期控管與現役金鑰查找等功能，
    /// 支援多金鑰並存（key rotation）、對稱與非對稱加密（RSA/AES）等應用場景。
    /// </summary>
    IJwtEncryptingKeyRepository JwtEncryptingKey { get; }
}
