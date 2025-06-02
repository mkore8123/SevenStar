using Common.Api.Authentication.Jwt;

namespace SevenStar.Shared.Domain.Api.Auth.Jwt;

public interface IJwtOptionsProvider
{
    Task<JwtOptions> GetBackendJwtOptionsAsync(int backendId);

    Task<JwtOptions> GetCompanyJwtOptionsAsync(int companyId);
}
