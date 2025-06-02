using SevenStar.Shared.Domain.Enums;
using Microsoft.Extensions.DependencyInjection;


namespace SevenStar.Shared.Domain.Service.Service;

public class DomainService : IDomainService
{
    private readonly IServiceProvider _provider;

    private readonly Lazy<IUserService> _user;

    public IUserService User => _user.Value;

    public DomainService(IServiceProvider provider)
    {
        _provider = provider;

        _user = new Lazy<IUserService>(
            () => _provider.GetRequiredKeyedService<IUserService>(MemberLevelEnum.Member),
            LazyThreadSafetyMode.ExecutionAndPublication
        );
    }
}
