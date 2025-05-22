using Common.Api.Option;

namespace SevenStar.Shared.Domain.Api.Token;

public interface IJwtOptionsFactory
{
    Task<JwtOptions> GetAsync(int companyId);
}
