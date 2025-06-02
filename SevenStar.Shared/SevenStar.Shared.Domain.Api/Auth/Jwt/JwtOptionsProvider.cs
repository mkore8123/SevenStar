using Common.Api.Auth.Jwt;
using Common.Api.Authentication.Jwt;
using Microsoft.IdentityModel.Tokens;
using SevenStar.Shared.Domain.DbContext.Platform;

namespace SevenStar.Shared.Domain.Api.Auth.Jwt;

public class JwtOptionsProvider : IJwtOptionsProvider
{
    private readonly IPlatformDb _platformDb;
    private readonly IServiceProvider _provider;

    public JwtOptionsProvider(IServiceProvider provider, IPlatformDb platformDb)
    {
        _provider = provider;
        _platformDb = platformDb;
    }

    public Task<JwtOptions> GetBackendJwtOptionsAsync(int backendId)
    {
        var jwtOptions = new JwtOptions
        {
            // 多組合法發行者，沒設定就不驗證
            ValidIssuers = new List<string> { "https://auth.example.com", "https://backup.example.com" },

            // 多組合法接收者，沒設定就不驗證
            ValidAudiences = new List<string> { "myapi", "mymobileapp" },

            // 簽章金鑰，一般會放多組供輪替（KeyId 要唯一，且對應 JWT header 的 kid）
            SigningKeys = new List<JwtSigningKeyOption>
            {
                new JwtSigningKeyOption
                {
                    Key = "super_secret_32_bytes_key_001",   // 應為高強度金鑰！
                    KeyId = "20240602_main"                  // 建議以日期+版本、GUID…唯一命名
                },
                new JwtSigningKeyOption
                {
                    Key = "super_secret_32_bytes_key_002",
                    KeyId = "20240610_backup"
                }
            },

            RequireSignedTokens = true,
            TryAllIssuerSigningKeys = true,

            // 時效相關
            RequireExpirationTime = true,
            ClockSkew = TimeSpan.FromMinutes(2),
            MaxTokenAge = TimeSpan.FromHours(2),   // Token 最長 2 小時，null 則不強制

            // 若啟用 JWE 可設解密金鑰，否則可為 null
            TokenDecryptionKeys = null,

            // claims 映射
            NameClaimType = "name",       // 對應 ClaimsPrincipal.Identity.Name
            RoleClaimType = "role",       // 對應角色判斷

            // 安全性選項
            EnableReplayDetection = false,   // 若啟用，需額外設 Replay Cache
            SaveSigninToken = false,

            // 自訂驗證類型
            AuthenticationType = "jwt",

            // 指定簽章演算法（常見：SecurityAlgorithms.HmacSha256）
            SigningAlgorithm = SecurityAlgorithms.HmacSha256
        };

        return Task.FromResult(jwtOptions);
    }

    public async Task<JwtOptions> GetCompanyJwtOptionsAsync(int companyId)
    {
        if (companyId <= 0)
            throw new ArgumentOutOfRangeException(nameof(companyId));

        var companyJwtEntity = await _platformDb.GetCompanyJwtOptions(companyId);

        var jwtOptions = new JwtOptions
        {
            // 多組合法發行者，沒設定就不驗證
            ValidIssuers = new List<string> { "https://auth.example.com", "https://backup.example.com" },

            // 多組合法接收者，沒設定就不驗證
            ValidAudiences = new List<string> { "myapi", "mymobileapp" },

            // 簽章金鑰，一般會放多組供輪替（KeyId 要唯一，且對應 JWT header 的 kid）
            SigningKeys = new List<JwtSigningKeyOption>
            {
                new JwtSigningKeyOption
                {
                    Key = "super_secret_32_bytes_key_001",   // 應為高強度金鑰！
                    KeyId = "20240602_main"                  // 建議以日期+版本、GUID…唯一命名
                },
                new JwtSigningKeyOption
                {
                    Key = "super_secret_32_bytes_key_002",
                    KeyId = "20240610_backup"
                }
            },

            RequireSignedTokens = true,
            TryAllIssuerSigningKeys = true,

            // 時效相關
            RequireExpirationTime = true,
            ClockSkew = TimeSpan.FromMinutes(2),
            MaxTokenAge = TimeSpan.FromHours(2),   // Token 最長 2 小時，null 則不強制

            // 若啟用 JWE 可設解密金鑰，否則可為 null
            TokenDecryptionKeys = null,

            // claims 映射
            NameClaimType = "name",       // 對應 ClaimsPrincipal.Identity.Name
            RoleClaimType = "role",       // 對應角色判斷

            // 安全性選項
            EnableReplayDetection = false,   // 若啟用，需額外設 Replay Cache
            SaveSigninToken = false,

            // 自訂驗證類型
            AuthenticationType = "jwt",

            // 指定簽章演算法（常見：SecurityAlgorithms.HmacSha256）
            SigningAlgorithm = SecurityAlgorithms.HmacSha256
        };

        return await Task.FromResult(jwtOptions);
    }
}