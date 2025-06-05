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
}
