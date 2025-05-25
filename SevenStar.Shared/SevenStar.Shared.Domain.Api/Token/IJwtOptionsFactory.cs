using Common.Api.Option;

namespace SevenStar.Shared.Domain.Api.Token;

public interface IJwtOptionsFactory
{
    Task<JwtOptions> GetBackendJwtOptionsAsync(int backendId);

    Task<JwtOptions> GetCompanyJwtOptionsAsync(int companyId);
}
