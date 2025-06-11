using Common.Api.Auth.Jwt;
using Common.Api.Authen.Jwt.@interface;
using Common.Api.Authentication;
using Jose;
using Microsoft.IdentityModel.Tokens;

namespace Common.Api.Authen.Jwt;

public class NestedJwsJweTokenService<TModel> : ITokenService<TModel>
{
    private readonly JwsTokenService<TModel> _jwsService;
    private readonly IJwtTokenConfigProvider<TModel> _configProvider;
    private readonly IJweEncryptingKeyProvider _encryptingKeyProvider;

    public NestedJwsJweTokenService(
        JwsTokenService<TModel> jwsService,
        IJwtTokenConfigProvider<TModel> configProvider,
        IJweEncryptingKeyProvider encryptingKeyProvider)
    {
        _jwsService = jwsService;
        _configProvider = configProvider;
        _encryptingKeyProvider = encryptingKeyProvider;
    }

    /// <summary>
    /// 產生 Nested JWT（先用 JWS 產生簽章，再包裝成 JWE）
    /// </summary>
    public async Task<string> GenerateToken(TModel model)
    {
        // 1. 直接交給 JwsTokenService 產生 JWS 字串
        var jws = await _jwsService.GenerateToken(model);

        // 2. 查詢加密用參數
        var cfg = await _configProvider.GetForModelAsync(model);
        var encryptingCredentials = await _encryptingKeyProvider.GetEncryptingCredentialsAsync(cfg.Issuer, cfg.Audience, cfg.JweKeyId);
        if (encryptingCredentials == null)
            throw new InvalidOperationException("未取得加密金鑰");

        var joseAlg = JweTokenService<TModel>.MapToJweAlgorithm(encryptingCredentials.Alg);
        var joseEnc = JweTokenService<TModel>.MapToJweEncryption(encryptingCredentials.Enc);
        var encKey = GetJoseKey(encryptingCredentials.Key);

        // 3. Header 組裝（cty="JWT" 代表 Nested）
        var header = new Dictionary<string, object>
        {
            ["alg"] = encryptingCredentials.Alg,
            ["enc"] = encryptingCredentials.Enc,
            ["cty"] = "JWT"
        };

        if (!string.IsNullOrWhiteSpace(cfg.JweKeyId))
            header["kid"] = cfg.JweKeyId;

        if (cfg.ExtraHeader is { Count: > 0 })
            foreach (var kv in cfg.ExtraHeader)
                header[kv.Key] = kv.Value;

        // 4. 用 JOSE 將 JWS 當 payload 加密成 JWE
        var jwe = JWT.Encode(jws, encKey, joseAlg, joseEnc, extraHeaders: header);

        return jwe;
    }

    /// <summary>
    /// 解密 JWE 外層，並交給 JwsTokenService 驗簽還原 model
    /// </summary>
    public async Task<TModel> DecrypteToken(string jwt)
    {
        // 1. 查詢解密參數
        var cfg = await _configProvider.GetForTokenAsync(jwt);
        var decryptKey = await _encryptingKeyProvider.GetDecryptingKeyAsync(cfg.Issuer, cfg.Audience, cfg.JweKeyId);

        var joseAlg = JweTokenService<TModel>.MapToJweAlgorithm(cfg.JweEncryptAlgorithm!);
        var joseEnc = JweTokenService<TModel>.MapToJweEncryption(cfg.JweContentEncryptAlgorithm!);

        string jws;
        try
        {
            jws = JWT.Decode(jwt, decryptKey, joseAlg, joseEnc);
        }
        catch (System.Exception ex)
        {
            throw new SecurityTokenException("JWE 解密失敗: " + ex.Message, ex);
        }

        // 2. 驗簽 & 解析 JWS 交給 JwsTokenService
        return await _jwsService.DecrypteToken(jws);
    }

    private static object GetJoseKey(SecurityKey key)
    {
        return key switch
        {
            RsaSecurityKey rsaKey => rsaKey.Rsa ?? RSAUtils.FromPem(rsaKey.PrivateKey!), // 需自備 RSAUtils.FromPem
            SymmetricSecurityKey symKey => symKey.Key,
            _ => throw new NotSupportedException("不支援的金鑰型別: " + key.GetType().Name)
        };
    }
}
