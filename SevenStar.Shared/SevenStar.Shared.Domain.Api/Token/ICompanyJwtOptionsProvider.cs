using Common.Api.Option;

namespace SevenStar.Shared.Domain.Api.Token;

public interface ICompanyJwtOptionsProvider
{
    Task<JwtOptions> GetAsync(int companyId);
}
