using Common.Api.Localization;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace SevenStar.Common.Api.Localization;

public class SevenStarStringLocalizer : BaseStringLocalizer
{
    /// <summary>
    /// 建構函式
    /// </summary>
    /// <param name="getCulture"></param>
    public SevenStarStringLocalizer(Func<CultureInfo> getCulture) : base(getCulture)
    {
    }

    /// <summary>
    /// 更新多語系來源實例
    /// </summary>
    /// <param name="newData"></param>
    public override void Refresh()
    {
        
    }
}
