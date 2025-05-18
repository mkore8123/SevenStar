using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Api.Token.Jwt.Model;

public class RefreshTokenModel
{
    public string Token { get; set; } = default!;

    public string UserId { get; set; } = default!;
    
    public string DeviceId { get; set; } = default!;
    
    public DateTime ExpiresAt { get; set; }
    
    public bool IsRevoked { get; set; } = false;
}
