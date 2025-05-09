using Common.Attributes;
using Microsoft.Extensions.DependencyInjection;
using SevenStar.Shared.Domain.Enums;
using SevenStar.Shared.Domain.Interface;

namespace SevenStar.Shared.Domain.Imp.Implement;

[KeyedService(MemberLevel.Lv1, ServiceLifetime.Scoped)]
public class Lv1Discount : IRebateService
{
    public decimal CalculateRebate(decimal totalAmount) => totalAmount * 0.05m;
}

[KeyedService(MemberLevel.Lv2, ServiceLifetime.Scoped)]
public class Lv2Discount : IRebateService
{
    public decimal CalculateRebate(decimal totalAmount) => totalAmount * 0.10m;
}

[KeyedService(MemberLevel.Lv3, ServiceLifetime.Scoped)]
public class Lv3Discount : IRebateService
{
    public decimal CalculateRebate(decimal totalAmount) => totalAmount * 0.20m;
}
