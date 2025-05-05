using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Common.Api.Localization;

public class BaseStringLocalizerFactory : IStringLocalizerFactory
{
    protected IStringLocalizer _localizer;

    public BaseStringLocalizerFactory()
    {
        _localizer = new BaseStringLocalizer(() => CultureInfo.CurrentUICulture);
    }

    /// <summary>
    /// 建立 IStringLocalizer 實例
    /// </summary>
    /// <param name="resourceSource"></param>
    /// <returns></returns>
    public IStringLocalizer Create(Type resourceSource) => _localizer;

    /// <summary>
    /// 建立 IStringLocalizer 實例
    /// </summary>
    /// <param name="baseName"></param>
    /// <param name="location"></param>
    /// <returns></returns>
    public IStringLocalizer Create(string baseName, string location) => _localizer;

    /// <summary>
    /// 取得目前的翻譯實例
    /// </summary>
    /// <returns></returns>
    public IStringLocalizer GetLocalizer() => _localizer;
}

