using Common.Api.Localization;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace SevenStar.Common.Api.Localization;

public class SevenStarStringLocalizerFactory : BaseStringLocalizerFactory
{
    /// <summary>
    /// 建構函式，可準備 SevenStarStringLocalizer 的相關配置
    /// </summary>
    public SevenStarStringLocalizerFactory(/*其他注入的服務*/)
    {
        _localizer = new SevenStarStringLocalizer(() => CultureInfo.CurrentUICulture);
    }
}

