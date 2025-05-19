using Common.Api.Token.Jwt.Model;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Json; // Add this namespace for System.Text.Json

namespace Common.Api.Token.Jwt;

public class RefreshTokenRedisService : ITokenService<RefreshTokenModel>
{
    private readonly IDatabase _redis;
    private readonly TimeSpan _refreshTokenTtl = TimeSpan.FromDays(7);
    private const string RedisKeyPrefix = "refresh_token:";

    public RefreshTokenRedisService(IConnectionMultiplexer connection)
    {
        _redis = connection.GetDatabase();
    }

    private static string GenerateSecureToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }

    private static string BuildRedisKey(string token)
    {
        return $"{RedisKeyPrefix}{token}";
    }

    public string GenerateToken(RefreshTokenModel model)
    {
        if (string.IsNullOrWhiteSpace(model.UserId))
            throw new ArgumentException("UserId 必須指定");

        var token = GenerateSecureToken();
        model.Token = token;
        model.ExpiresAt = DateTime.UtcNow.Add(_refreshTokenTtl);

        var redisKey = BuildRedisKey(token);
        var json = JsonSerializer.Serialize(model); // Use System.Text.Json.JsonSerializer
        _redis.StringSet(redisKey, json, _refreshTokenTtl);

        return token;
    }

    public RefreshTokenModel DecrypteToken(string jwtToken)
    {
        var redisKey = BuildRedisKey(jwtToken);
        var value = _redis.StringGet(redisKey);
        if (!value.HasValue)
            throw new SecurityTokenException("Refresh Token 無效");

        var model = JsonSerializer.Deserialize<RefreshTokenModel>(value!); // Use System.Text.Json.JsonSerializer
        if (model == null || model.IsRevoked || model.ExpiresAt < DateTime.UtcNow)
            throw new SecurityTokenException("Refresh Token 已過期或被撤銷");

        return model;
    }

    public RefreshTokenModel ExtractModelFromClaims(ClaimsPrincipal principal)
    {
        throw new NotImplementedException();
    }

    public List<Claim> BuildClaimsFromModel(RefreshTokenModel model)
    {
        throw new NotImplementedException();
    }
}
