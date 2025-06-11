using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Api.Authen.Jwt;

public class JwtEncryptingKey
{
    public int Id { get; set; }
    public int ConfigId { get; set; }
    public string KeyId { get; set; } = default!;
    public string Algorithm { get; set; } = default!;      // 如 RSA-OAEP, A256KW...
    public string ContentAlg { get; set; } = default!;     // 如 A256GCM, A256CBC-HS512
    public string? PublicKey { get; set; }
    public string? PrivateKey { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
    public bool IsActive { get; set; }
}
