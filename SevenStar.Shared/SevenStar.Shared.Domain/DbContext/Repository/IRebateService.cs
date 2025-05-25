using System;
using System.Collections.Generic;
using System.Text;

namespace SevenStar.Shared.Domain.DbContext.Repository;

public interface IRebateService
{
    decimal CalculateRebate(decimal totalAmount);
}