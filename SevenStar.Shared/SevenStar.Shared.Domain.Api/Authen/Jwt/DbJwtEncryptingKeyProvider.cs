using Common.Api.Authen.Jwt;
using Common.Api.Authen.Jwt.Interface;
using Common.Api.Authen.Jwt.Model;
using Dapper;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace SevenStar.Shared.Domain.Api.Authen.Jwt;

public class DbJweEncryptingKeyProvider : IJweEncryptingKeyProvider
{
    private readonly IApiSingletonCacheService _cacheService;

    public DbJweEncryptingKeyProvider(IApiSingletonCacheService cacheService)
    {
        _cacheService = cacheService;
    }

    public async Task<JwtEncryptingKey?> GetAvailableKeyAsync(string issuer, string audience)
    {
        var encryptingKey = _cacheService.Jwt.GetJweEncryptingForIssue(issuer, audience);
        return await Task.FromResult(encryptingKey) ?? throw new InvalidOperationException($"No signing key found for issuer '{issuer}' and audience '{audience}'.");
    }

    public async Task<JwtEncryptingKey?> GetKeyAsync(string issuer, string audience, string keyId)
    {
        var encryptingKey = _cacheService.Jwt.GetJweEncryptingForValidate(issuer, audience, keyId);
        return await Task.FromResult(encryptingKey) ?? throw new InvalidOperationException($"No signing key found for issuer '{issuer}' and audience '{audience}'.");
    }
}