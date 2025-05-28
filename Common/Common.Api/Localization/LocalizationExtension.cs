using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using System.Globalization;

namespace Common.Api.Localization;

/// <summary>
/// Web Api 自訂啟用 多語系 流程
/// </summary>
public static class LocalizationExtension
{
    /// <summary>
    /// 註冊多語系相關配置
    /// </summary>
    public static IServiceCollection AddLocalizationHandler(this IServiceCollection services, LocalizationBase localizationBase)
    {
        var factory = localizationBase.GetLocalizerFactory();
        services.AddSingleton(factory);
        services.AddSingleton(factory); // 用來手動更新資料

        services.AddLocalization();
        services.Configure<RequestLocalizationOptions>(options => localizationBase.ConfigLocalizationOptions(options));

        return services;
    }

    /// <summary>
    /// 中介層使用 Swagger
    /// </summary>
    public static IApplicationBuilder UseLocalizationHandling(this IApplicationBuilder app)
    {
        app.UseRequestLocalization();

        return app;
    }
}