using Common.Attributes;
using Microsoft.Extensions.DependencyInjection;
using SevenStar.Shared.Domain.Enums;
using SevenStar.Shared.Domain.Repository;

namespace SevenStar.Shared.Domain.Imp.Implement;

[KeyedService(MemberLevelEnum.Lv1, ServiceLifetime.Scoped)]
public class Lv1Discount : IRebateService
{
    public decimal CalculateRebate(decimal totalAmount) => totalAmount * 0.05m;
}

[KeyedService(MemberLevelEnum.Lv2, ServiceLifetime.Scoped)]
public class Lv2Discount : IRebateService
{
    public decimal CalculateRebate(decimal totalAmount) => totalAmount * 0.10m;
}

[KeyedService(MemberLevelEnum.Lv3, ServiceLifetime.Scoped)]
public class Lv3Discount : IRebateService
{
    public decimal CalculateRebate(decimal totalAmount) => totalAmount * 0.20m;
}
