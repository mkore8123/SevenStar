using Common.Api.Auth.Jwt;
using Common.Api.Authen.Enum;
using Common.Api.Authen.Jwt.Interface;
using Common.Api.Authentication;
using Jose;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Reflection.PortableExecutable;

namespace Common.Api.Authen.Jwt;

public class NestedJwsJweTokenService<TModel> : ITokenService<TModel>
{
    private readonly ITokenService<TModel> _jwsService;
    private readonly IJwtTokenConfigProvider<TModel> _configProvider;
    private readonly IJweEncryptingKeyProvider _encryptingKeyProvider;

    public NestedJwsJweTokenService(
        [FromKeyedServices(JwtEnvelopeType.Jws)] ITokenService<TModel> jwsService,
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
        
        var key = await _encryptingKeyProvider.GetAvailableKeyAsync(cfg.Issuer, cfg.Audience);
        if (key == null)
            throw new InvalidOperationException("未取得加密金鑰");

        var joseAlg = JweTokenService<TModel>.MapToJweAlgorithm(key.Algorithm);
        var joseEnc = JweTokenService<TModel>.MapToJweEncryption(key.ContentAlgorithm);
        var encryptingKey = JweTokenService<TModel>.GetJoseEncryptingKey(key);

        // 3. Header 組裝（cty="JWT" 代表 Nested）
        var header = new Dictionary<string, object>
        {
            ["alg"] = joseAlg,
            ["enc"] = joseEnc,
            ["cty"] = "JWT"  // 巢狀或複合 JWE，表示內層是 JWS
        };

        if (!string.IsNullOrWhiteSpace(key.KeyId))
            header["kid"] = key.KeyId;

        if (cfg.ExtraHeader is { Count: > 0 })
            foreach (var kv in cfg.ExtraHeader)
                header[kv.Key] = kv.Value;

        // 4. 用 JOSE 將 JWS 當 payload 加密成 JWE
        var jwe = JWT.Encode(jws, encryptingKey, joseAlg, joseEnc, extraHeaders: header);

        return jwe;
    }

    /// <summary>
    /// 解密 JWE 外層，並交給 JwsTokenService 驗簽還原 model
    /// </summary>
    public async Task<TModel> DecrypteToken(string jwt)
    {
        // 先解 header，取得 iss/aud/kid
        var headers = JWT.Headers(jwt);

        // 1. 查詢解密參數
        var cfg = await _configProvider.GetForTokenAsync(jwt);
        var kid = headers.TryGetValue("kid", out var kidObj) ? kidObj?.ToString() ?? "" : "";

        var decryptKey = await _encryptingKeyProvider.GetKeyAsync(cfg.Issuer, cfg.Audience, kid);

        var joseAlg = JweTokenService<TModel>.MapToJweAlgorithm(decryptKey.Algorithm);
        var joseEnc = JweTokenService<TModel>.MapToJweEncryption(decryptKey.ContentAlgorithm);

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
}
