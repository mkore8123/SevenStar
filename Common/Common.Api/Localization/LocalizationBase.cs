using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Localization.Routing;
using Microsoft.Extensions.Configuration;
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

    /// <summary>
    /// 建立並返回一個 IRequestCultureProvider 的清單，用於在 ASP.NET Core 應用程式中判斷每個 HTTP 請求所應用的文化（Culture）與 UI 文化（UICulture）。
    /// 這些提供者會依序執行，直到其中一個成功解析出文化資訊為止
    /// </summary>
    /// <returns></returns>
    public virtual List<IRequestCultureProvider> GetRequestCultureProvider()
    {
        var requestCultureProvider = new List<IRequestCultureProvider>
        {
            // 從路由資料（RouteData）中讀取文化設定。適用於在 URL 路徑中包含語言代碼的情境，例如：https://example.com/zh-TW/home/index。
            new RouteDataRequestCultureProvider(),

            // 從名為 .AspNetCore.Culture 的 Cookie 中讀取文化設定。
            // Cookie 的值通常為 c=zh-TW|uic=zh-TW，其中 c 表示 Culture，uic 表示 UICulture。
            new CookieRequestCultureProvider(),

            // 從 URL 的查詢字串中讀取文化設定。URL 範例如下：https://example.com/?culture=zh-TW&ui-culture=zh-TW。
            // 適用於需要透過 URL 明確指定語言的情境，例如語言切換連結或特定語言的分享連結。若僅提供 culture 或 ui-culture 其中之一，系統會將該值同時套用於 Culture 與 UICulture。
            new QueryStringRequestCultureProvider(),

            // 從 HTTP 請求的 Accept-Language 標頭中讀取文化設定。此標頭通常由瀏覽器根據使用者的語言偏好自動設定。
            // 適用於首次訪問網站時，根據使用者的瀏覽器語言偏好自動選擇適當的語言。
            new AcceptLanguageHeaderRequestCultureProvider()
        };

        return requestCultureProvider;
    }

    /// <summary>
    /// 配置多語系的參數選項
    /// </summary>
    /// <param name="options"></param>
    public void ConfigLocalizationOptions(Microsoft.AspNetCore.Builder.RequestLocalizationOptions options)
    {
        var defaultCultures = GetSupportedCultures();
        options.DefaultRequestCulture = new RequestCulture(GetDefaultCulture());
        options.SupportedCultures = defaultCultures;
        options.SupportedUICultures = defaultCultures;

        // 此選項決定是否將目前的 CultureInfo.CurrentUICulture 套用至回應的 Content-Language 標頭。預設為 false。
        options.ApplyCurrentCultureToResponseHeaders = true;

        // 此選項決定是否使用使用者在作業系統中設定的文化特定格式（例如日期和數字格式）。預設為 true。
        options.CultureInfoUseUserOverride = true;

        // 這兩個選項控制當指定的文化未被支援時，是否回退至其父文化。預設皆為 true。
        // 例如，若使用者請求 fr-CA（加拿大法語），但應用程式僅支援 fr（法語），則會回退至 fr。
        options.FallBackToParentCultures = true;
        options.FallBackToParentUICultures = true;

        // 這些提供者的順序會影響到文化的選擇
        options.RequestCultureProviders = GetRequestCultureProvider();
    }
}
