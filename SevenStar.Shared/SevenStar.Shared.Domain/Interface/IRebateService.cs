using System;
using System.Collections.Generic;
using System.Text;

namespace SevenStar.Shared.Domain.Interface;

public interface IRebateService
{
    decimal CalculateRebate(decimal totalAmount);
}