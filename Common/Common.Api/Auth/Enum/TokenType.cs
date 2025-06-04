using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Api.Auth.Enum;

public enum TokenType
{
    Jwt,
    Cookie,
    ApiKey,
    Opaque
}