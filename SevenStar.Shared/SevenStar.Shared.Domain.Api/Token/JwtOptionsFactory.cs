using Common.Api.Option;
using Microsoft.IdentityModel.Tokens;
using SevenStar.Shared.Domain.Database;
using System;
using System.Collections.Generic;
using System.Text;

namespace SevenStar.Shared.Domain.Api.Token;

public class JwtOptionsFactory : IJwtOptionsFactory
{
    private readonly IServiceProvider _provider;
    private readonly IPlatformDb _platformDb;

    public JwtOptionsFactory(IPlatformDb platformDb)
    {
        _platformDb = platformDb;
    }

    public Task<JwtOptions> GetAsync(int companyId)
    {
        return Task.FromResult(options);
    }
}