using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using SevenStar.ApiService.Controllers;
using SevenStar.Shared.Domain.DbContext.Company.Repository;
using SevenStar.Shared.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SevenStar.Shared.Domain.Api.Controllers;

public class SevenStarController : ApiControllerBase
{

    public SevenStarController()
    {

    }

    [HttpGet("apply-discount")]
    public IActionResult ApplyDiscount(decimal amount)
    {
        // var discountedAmount = _discountService.CalculateRebate(amount);
        return Ok("ok");
    }
}
