using Common.Api.Authen.Jwt.Interface;
using Common.Api.Authen.Jwt.Model;
using Microsoft.IdentityModel.Tokens;
using SevenStar.Shared.Domain.Api.Auth.Jwt;
using SevenStar.Shared.Domain.DbContext.Platform;
using System.Text;

namespace SevenStar.Shared.Domain.Api.Authen.Jwt;

public class DbJwtSigningKeyProvider : IJwtSigningKeyProvider
{
    private readonly IApiSingletonCacheService _cacheService;

    public DbJwtSigningKeyProvider(IApiSingletonCacheService cacheService)
    {
        _cacheService = cacheService;
    }

    public async Task<JwtSigningKey> GetAvailableKeyAsync(string issuer, string audience)
    {
        var signingKey = _cacheService.Jwt.GetJwsSigningForIssue(issuer, audience);
        return await Task.FromResult(signingKey) ?? throw new InvalidOperationException($"No signing key found for issuer '{issuer}' and audience '{audience}'.");
    }

    public async Task<JwtSigningKey> GetKeyAsync(string issuer, string audience, string keyId)
    {
        var signingKey = _cacheService.Jwt.GetJwsSigningForValidate(issuer, audience, keyId);
        return await Task.FromResult(signingKey) ?? throw new InvalidOperationException($"No signing key found for issuer '{issuer}' and audience '{audience}'.");
    }
}
