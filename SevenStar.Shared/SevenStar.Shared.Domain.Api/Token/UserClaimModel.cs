using System;
using System.Collections.Generic;
using System.Text;

namespace SevenStar.Shared.Domain.Api.Token;

public class UserClaimModel
{
    public long UserId { get; set; } = 0;

    public string TokenVersion { get; set; } = string.Empty;

    public string UserName { get; set; } = string.Empty;
    
    public string Email { get; set; } = string.Empty;
    
    public List<string> Roles { get; set; } = new List<string>();
}
