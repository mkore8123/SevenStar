using Common.Api.Option;
using Common.Api.Token.Jwt.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Common.Api.Token.Jwt;

public abstract class JwtTokenServiceBase<TModel> : IJwtMetadataService, ITokenService<TModel>
{
    private readonly JwtOptions _options;
    private readonly JwtSecurityTokenHandler _tokenHandler = new();

    protected JwtTokenServiceBase(JwtOptions options)
    {
        _options = options;
    }

    /// <summary>
    /// 解密 JWT Token 以取得 Model
    /// </summary>
    /// <param name="principal"></param>
    /// <returns></returns>
    protected abstract TModel ExtractModelFromClaims(ClaimsPrincipal principal);

    /// <summary>
    /// 壓縮生成  JWT Token 所需的 Claims
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    protected abstract List<Claim> BuildClaimsFromModel(TModel model);

    /// <summary>
    /// 建立 JwtBearerEvents，可在 DI 註冊 JwtBearerOptions 時使用
    /// </summary>
    /// <param name="serviceProvider">目前的 DI 容器</param>
    /// <returns>自定義事件處理器</returns>
    public abstract JwtBearerEvents CreateJwtBearerEvents();


    public string GenerateToken(TModel model)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Secret));
        var creds = new SigningCredentials(key, _options.Algorithms);

        var claims = BuildClaimsFromModel(model);

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_options.AccessTokenExpirationMinutes),
            signingCredentials: creds
        );

        return _tokenHandler.WriteToken(token);
    }

    public TModel DecrypteToken(string jwtToken)
    {
        try
        {
            var principal = GetPrincipalFromToken(jwtToken);
            return ExtractModelFromClaims(principal);
        }
        catch (SecurityTokenException ex)
        {
            throw new UnauthorizedAccessException("Token 驗證失敗", ex);
        }
    }

    private ClaimsPrincipal GetPrincipalFromToken(string jwtToken)
    {
        var principal = _tokenHandler.ValidateToken(jwtToken, CreateValidationParameters(), out var validatedToken);

        if (validatedToken is not JwtSecurityToken jwt ||
            !jwt.Header.Alg.Equals(_options.Algorithms, StringComparison.OrdinalIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token algorithm.");
        }

        return principal;
    }

    public TokenValidationParameters CreateValidationParameters()
    {
        return new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Secret)),

            ValidateIssuer = true,
            ValidIssuer = _options.Issuer,

            ValidateAudience = true,
            ValidAudience = _options.Audience,

            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    }
}
