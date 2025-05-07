using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Localization;
using SevenStar.Common.Api.Localization;
using System.Globalization;

namespace Common.Api.Localization;

/// <summary>
/// 根據指定系統的多語系需求，實作 IStringLocalizerFactory 介面以提供 IStringLocalizer 實作的工廠類別
/// </summary>
public class SevenStarLocalization : LocalizationBase
{
    /// <summary>
    /// 建構函式
    /// </summary>
    public SevenStarLocalization() {}

    /// <summary>
    /// 指定使用的 IStringLocalizerFactory 型別，隨著註冊後會啟用該基礎類別的 IStringLocalizerFactory.Create() 方法，創建 IStringLocalizer 實例
    /// </summary>
    public override IStringLocalizerFactory GetLocalizerFactory()
    {
        return new SevenStarStringLocalizerFactory();
    }

    /// <summary>
    /// 指定預設語系
    /// </summary>
    public override CultureInfo GetDefaultCulture()
    {
        return new CultureInfo("zh-TW");
    }

    /// <summary>
    /// 取得所有支援的語系清單
    /// </summary>
    public override List<CultureInfo> GetSupportedCultures()
    {
        return new[]
        {
            new CultureInfo("zh-TW"),
            new CultureInfo("en-US"),
        }.ToList();
    }

    public override List<IRequestCultureProvider> GetRequestCultureProvider()
    {
        return new List<IRequestCultureProvider>() { new CookieRequestCultureProvider() };
    }
}
