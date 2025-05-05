using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Common.Api.Localization;

public class BaseStringLocalizer : IStringLocalizer
{
    private readonly Func<CultureInfo> _getCulture;
    private Dictionary<string, Dictionary<string, string>> _data = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// 建構函式
    /// </summary>
    /// <param name="getCulture"></param>
    public BaseStringLocalizer(Func<CultureInfo> getCulture)
    {
        _getCulture = getCulture;
    }

    /// <summary>
    /// 更新多語系來源實例
    /// </summary>
    /// <param name="newData"></param>
    public virtual void UpdateData(Dictionary<string, Dictionary<string, string>> newData)
    {
        var newCache = new Dictionary<string, Dictionary<string, string>>(newData, StringComparer.OrdinalIgnoreCase);
        Interlocked.Exchange(ref _data, newCache);
    }

    /// <summary>
    /// 複寫 Refresh 方法以自訂取得資料來源邏輯，讓使用者可以重新載入多語系資料
    /// </summary>
    public virtual void Refresh()
    {
        var newData = new Dictionary<string, Dictionary<string, string>>();
        UpdateData(newData);
    }

    /// <summary>
    /// 進行翻譯方法
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public LocalizedString this[string name]
    {
        get
        {
            var culture = _getCulture();
            var value = TryGetString(name, culture.Name, out var found) ? found : name;
            return new LocalizedString(name, value, resourceNotFound: value == name);
        }
    }

    public LocalizedString this[string name, params object[] arguments]
        => new(name, string.Format(this[name].Value, arguments), resourceNotFound: false);

    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
    {
        var culture = _getCulture();
        if (_data.TryGetValue(culture.Name, out var dict))
        {
            return dict.Select(kv => new LocalizedString(kv.Key, kv.Value, false));
        }

        return Enumerable.Empty<LocalizedString>();
    }

    private bool TryGetString(string key, string culture, out string value)
    {
        value = key;
        return _data.TryGetValue(culture, out var dict) && dict.TryGetValue(key, out value);
    }
}
