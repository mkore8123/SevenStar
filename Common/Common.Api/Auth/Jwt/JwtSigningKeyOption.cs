using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Api.Auth.Jwt;

public class JwtSigningKeyOption
{
    /// <summary>
    /// 金鑰內容（建議 base64 或原始字串）
    /// </summary>
    public string Key { get; set; } = default!;

    /// <summary>
    /// 對應 JWT header 的 kid，必須唯一
    /// </summary>
    public string KeyId { get; set; } = default!;
}
