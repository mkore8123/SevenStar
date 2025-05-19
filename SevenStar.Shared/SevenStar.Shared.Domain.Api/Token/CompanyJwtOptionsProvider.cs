using Common.Api.Option;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace SevenStar.Shared.Domain.Api.Token;

public class CompanyJwtOptionsProvider : ICompanyJwtOptionsProvider
{

    public CompanyJwtOptionsProvider()
    {
    }

    public Task<JwtOptions> GetAsync(int companyId)
    {
        var options = new JwtOptions()
        {
            Secret = "m9U4f!2F@1Kqz7R$Lw3TgVx8pY6bCnDz", 
            Issuer = "SevenStar.ApiService",          
            Audience = "SevenStar.FrontendApp",      
            Algorithms = SecurityAlgorithms.HmacSha256,
            AccessTokenExpirationMinutes = 60        
        };

        return Task.FromResult(options);
    }
}