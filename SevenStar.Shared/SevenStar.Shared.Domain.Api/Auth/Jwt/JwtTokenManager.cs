using Common.Api.Authentication.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SevenStar.Shared.Domain.Api.Auth.Jwt.Event;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SevenStar.Shared.Domain.Api.Auth.Jwt;

public class JwtTokenManager : JwtTokenServiceBase<UserClaimModel>
{
    private readonly IServiceProvider _provider;

    /// <summary>
    /// 密切注意不可注入 Scope 類型物件
    /// </summary>
    /// <param name="provider"></param>
    /// <param name="options"></param>
    public JwtTokenManager(IServiceProvider provider, JwtOptions options) : base(options) 
    {
        _provider = provider;
    }

    public override UserClaimModel ExtractModelFromClaims(ClaimsPrincipal principal)
    {
        var userIdClaim = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
            throw new SecurityTokenException("Required claim 'sub' is missing or null.");

        var userId = long.Parse(userIdClaim);

        var role = principal.FindFirst(ClaimTypes.Role)?.Value;
        if (string.IsNullOrEmpty(role))
            throw new SecurityTokenException("Required claim 'role' is missing or null.");

        var email = principal.FindFirst(ClaimTypes.Email)?.Value;
        var tokenVersion = principal.FindFirst("token_version")?.Value;

        var currentVersion = "0"; // 從 redis 取回該用戶的版本號
        if (tokenVersion != currentVersion)
        {
            throw new UnauthorizedAccessException("Token 已被撤銷");
        }

        return new UserClaimModel
        {
            UserId = userId,
            Email = email
        };
    }

    public override List<Claim> BuildClaimsFromModel(UserClaimModel model)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, model.UserId.ToString()),
            // new(ClaimTypes.Role, model.Role),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new("token_version", model.TokenVersion.ToString())
        };

        if (!string.IsNullOrWhiteSpace(model.Email))
            claims.Add(new Claim(ClaimTypes.Email, model.Email));

        return claims;
    }

    public override JwtBearerEvents CreateJwtBearerEvents()
    {
        return new JwtEventHandler(_provider, this);
    }
}