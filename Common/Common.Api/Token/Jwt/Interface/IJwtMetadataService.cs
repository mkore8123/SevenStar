using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Api.Token.Jwt.Interface;

public interface IJwtMetadataService
{
    TokenValidationParameters CreateValidationParameters();
}
