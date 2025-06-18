using Common.Api.Authen.Jwt.Enum;
using Common.Api.Authen.Jwt.Interface;
using Common.Api.Authentication;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Api.Authen.Jwt.Implement;

public class JwtTokenServiceFactory<TClaimModel> : IJwtTokenServiceFactory<TClaimModel>
{
    /// <summary>
    /// 根據 EnvelopeType 從 DI 解析對應的 TokenService
    /// </summary>
    public ITokenService<TClaimModel> Create(JwtEnvelopeType envelopeType, IServiceProvider services)
    {
        // 確認 services 不為 null
        if (services == null) throw new ArgumentNullException(nameof(services));

        // 嘗試以 envelopeType 當作 Key 從 DI 取對應的 TokenService
        try
        {
            var service = services.GetRequiredKeyedService<ITokenService<TClaimModel>>(envelopeType);
            if (service == null)
                throw new InvalidOperationException($"找不到對應 EnvelopeType:{envelopeType} 的 TokenService。");
            return service;
        }
        catch (System.Exception ex)
        {
            // 可考慮記錄日誌，或依需求拋出自訂例外
            throw new InvalidOperationException(
                $"解析 JwtEnvelopeType={envelopeType} 的 ITokenService<{typeof(TClaimModel).Name}> 失敗，請檢查 Keyed DI 註冊。",
                ex
            );
        }
    }
}

