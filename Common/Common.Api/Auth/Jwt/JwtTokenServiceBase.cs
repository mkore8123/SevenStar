using Common.Api.Auth.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Common.Api.Authentication.Jwt;

/// <summary>
/// JWT Token Service 基底類別，支援多組簽章金鑰、多 Issuer/Audience，專為高擴充性設計
/// </summary>
public abstract class JwtTokenServiceBase<TModel> : ITokenService<TModel>
{
    public readonly JwtOptions _options;
    private readonly JwtSecurityTokenHandler _tokenHandler = new();

    protected JwtTokenServiceBase(JwtOptions options)
    {
        _options = options;
    }

    /// <summary>
    /// 建立自訂 JwtBearerEvents，註冊 JwtBearerOptions 時可用於事件擴充（例如驗證失敗、接收 Token 等）
    /// </summary>
    public virtual JwtBearerEvents CreateJwtBearerEvents()
    {
        return new JwtBearerEvents();
    }

    /// <summary>
    /// 產生 JWT Token
    /// </summary>
    /// <param name="model">來源資料模型</param>
    /// <returns>JWT 字串</returns>
    public string GenerateToken(TModel model)
    {
        // 1. 產生 Token 前，確保 JwtOptions 已經通過嚴格驗證
        _options.Validate();

        // 2. 選用第一組簽章金鑰（如多租戶可自行調整選擇邏輯）
        var keyOption = _options.SigningKeys!.First();

        // 3. 建立對稱金鑰物件，並設定 KeyId
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyOption.Key))
        {
            KeyId = keyOption.KeyId
        };

        // 4. 建立簽章憑證
        var creds = new SigningCredentials(securityKey, _options.SigningAlgorithm);

        // 5. 動態決定 claims, issuer, audience
        var issuer = _options.ValidIssuers?.FirstOrDefault();
        var audience = _options.ValidAudiences?.FirstOrDefault();
        var claims = BuildClaimsFromModel(model);

        // 6. 產生有效期（僅有 MaxTokenAge 時才帶入 exp，否則不帶）
        var now = DateTime.UtcNow;
        DateTime? expires = _options.MaxTokenAge.HasValue ? now.Add(_options.MaxTokenAge.Value) : (DateTime?)null;
        DateTime? notBefore = now; // 可視需求帶入或設 null

        // 7. 建立 JwtPayload，未指定欄位則不會產生對應 claim
        var payload = new JwtPayload(
            issuer: issuer,
            audience: audience,
            claims: claims,
            notBefore: notBefore,
            expires: expires,
            issuedAt: now
        );

        // 8. 建立 JwtHeader，主動帶上 kid
        var header = new JwtHeader(creds)
        {
            { "kid", keyOption.KeyId }
        };

        // 9. 組合 Header 與 Payload，產生 Token
        var token = new JwtSecurityToken(header, payload);

        // 10. 回傳序列化後的 Token 字串
        return _tokenHandler.WriteToken(token);
    }

    /// <summary>
    /// 驗證並解密 JWT Token，取出對應模型資料
    /// </summary>
    /// <param name="jwtToken">JWT 字串</param>
    /// <returns>TModel</returns>
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

    /// <summary>
    /// 內部使用：取得 Token 驗證後的 ClaimsPrincipal
    /// </summary>
    /// <param name="jwtToken"></param>
    /// <returns></returns>
    private ClaimsPrincipal GetPrincipalFromToken(string jwtToken)
    {
        var principal = _tokenHandler.ValidateToken(jwtToken, _options.ToTokenValidationParameters(), out var validatedToken);

        if (validatedToken is not JwtSecurityToken jwt ||
            (_options.RequireSignedTokens && !jwt.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.OrdinalIgnoreCase)))
        {
            throw new SecurityTokenException("Invalid token algorithm or signature.");
        }

        return principal;
    }

    /// <summary>
    /// 將模型資料轉換為 JWT Claims
    /// </summary>
    public abstract List<Claim> BuildClaimsFromModel(TModel model);

    /// <summary>
    /// 從 ClaimsPrincipal 提取模型資料
    /// </summary>
    public abstract TModel ExtractModelFromClaims(ClaimsPrincipal principal);
}