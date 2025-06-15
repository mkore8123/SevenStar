using Common.Api.Authen.Jwt.Interface;
using Common.Api.Authen.Jwt.Model;
using System.Collections.Concurrent;

namespace SevenStar.Shared.Domain.Api;

public partial class ApiSingletonCacheService : IApiSingletonCacheService
{
    public IJwtCacheService Jwt { get; }

    public ApiSingletonCacheService(IJwtCacheService jwt)
    {
        Jwt = jwt ?? throw new ArgumentNullException(nameof(jwt), "JWT Cache Service cannot be null.");
    }
}

