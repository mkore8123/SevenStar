using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Api.Authen.Enum;

/// <summary>
/// JWT 應用場景類型（依 OAuth2/OIDC 等標準用途）
/// </summary>
public enum JwtPurposeType
{
    /// <summary>
    /// 存取權杖（Access Token，typ="at+jwt"）
    /// </summary>
    AccessToken,

    /// <summary>
    /// 身份權杖（ID Token，typ="id_token"）
    /// </summary>
    IdToken,

    /// <summary>
    /// 刷新權杖（Refresh Token）
    /// </summary>
    RefreshToken,

    /// <summary>
    /// 自訂/其他用途
    /// </summary>
    Custom
}
