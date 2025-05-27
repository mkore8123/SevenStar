using Common.Api.Option;

namespace SevenStar.Shared.Domain.Api.Token.Jwt;

public interface IJwtOptionsProvider
{
    Task<JwtOptions> GetBackendJwtOptionsAsync(int backendId);

    Task<JwtOptions> GetCompanyJwtOptionsAsync(int companyId);
}
