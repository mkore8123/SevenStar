using Common.Api.Authen.Enum;
using Common.Api.Authentication;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Api.Authen.Jwt.Interface;

/// <summary>
/// 封裝 ITokenService<T> 的工廠介面（方便測試、DI 注入與彈性）
/// </summary>
public interface IJwtTokenServiceFactory<TClaimModel>
{
    ITokenService<TClaimModel> Create(JwtEnvelopeType envelopeType, IServiceProvider services);
}