using Microsoft.Extensions.Localization;
using System.Globalization;

namespace Common.Api.Localization;

public class LocalizationBase
{
    /// <summary>
    /// 指定使用的 IStringLocalizerFactory 型別
    /// </summary>
    public virtual IStringLocalizerFactory GetLocalizerFactory()
    {
        return new BaseStringLocalizerFactory();
    }

    /// <summary>
    /// 指定預設語系
    /// </summary>
    public virtual CultureInfo GetDefaultCulture() => new CultureInfo("zh-TW");

    /// <summary>
    /// 回傳所有支援的語系清單
    /// </summary>
    public virtual List<CultureInfo> GetSupportedCultures()
    {
        return new[]
        {
            new CultureInfo("zh-TW"),
            new CultureInfo("en-US"),
        }.ToList();
    }
}
