using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Common.Api.Auth.Claims;

public class UserClaimModel
{
    public long UserId { get; set; } = default!;
    
    public string Device { get; set; } = default!; // mobile or web
}