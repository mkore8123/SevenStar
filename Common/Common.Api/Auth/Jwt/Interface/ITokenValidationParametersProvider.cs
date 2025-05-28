using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Api.Authentication.Jwt.Interface;

public interface ITokenValidationParametersProvider
{
    TokenValidationParameters CreateValidationParameters();
}
