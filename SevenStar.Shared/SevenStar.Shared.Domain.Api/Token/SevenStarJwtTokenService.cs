using StackExchange.Redis;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Common.Api.Option;
using Common.Api.Token.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace SevenStar.Shared.Domain.Api.Token;

public class SevenStarJwtTokenService : JwtTokenServiceBase<SampleMemberModel>
{
    private readonly IServiceProvider _provider;

    public SevenStarJwtTokenService(IServiceProvider provider, JwtOptions options) : base(options) 
    {
        _provider = provider;
    }

    protected override SampleMemberModel ExtractModelFromClaims(ClaimsPrincipal principal)
    {
        var userId = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        var role = principal.FindFirst(ClaimTypes.Role)?.Value;
        var email = principal.FindFirst(ClaimTypes.Email)?.Value;
        var tokenVersion = principal.FindFirst("token_version")?.Value;

        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(role))
            throw new SecurityTokenException("Required claims missing.");

        var currentVersion = "0"; // 從 redis 取回該用戶的版本號
        if (tokenVersion != currentVersion)
        {
            throw new UnauthorizedAccessException("Token 已被撤銷");
        }

        return new SampleMemberModel
        {
            UserId = userId,
            Role = role,
            Email = email
        };
    }

    protected override List<Claim> BuildClaimsFromModel(SampleMemberModel model)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, model.UserId),
            new(ClaimTypes.Role, model.Role),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new("token_version", model.TokenVersion.ToString())
        };

        if (!string.IsNullOrWhiteSpace(model.Email))
            claims.Add(new Claim(ClaimTypes.Email, model.Email));

        return claims;
    }

    public override JwtBearerEvents CreateJwtBearerEvents()
    {
        var redis = _provider.GetRequiredService<IConnectionMultiplexer>();

        return new JwtBearerEventsBase(redis);
    }
}