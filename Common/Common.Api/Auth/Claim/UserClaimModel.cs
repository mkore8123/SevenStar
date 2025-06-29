using Common.Enums;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Common.Api.Auth.Claims;

public class UserClaimModel
{
    public long Id { get; set; } = default!;
    
    public DeviceTypeEnum Device { get; set; } = default!;
}