using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using SevenStar.ApiService.Controllers;
using SevenStar.Shared.Domain.Enums;
using SevenStar.Shared.Domain.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace SevenStar.Shared.Domain.Api.Controllers;

public class SevenStarController : ApiControllerBase
{
    private readonly IRebateService _discountService;

    public SevenStarController([FromKeyedServices(MemberLevel.Lv1)] IRebateService discountService)
    {
        _discountService = discountService;
    }

    [HttpGet("apply-discount")]
    public IActionResult ApplyDiscount(decimal amount)
    {
        var discountedAmount = _discountService.CalculateRebate(amount);
        return Ok(discountedAmount);
    }
}
