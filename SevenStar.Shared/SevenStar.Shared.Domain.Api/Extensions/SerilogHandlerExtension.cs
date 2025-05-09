using Serilog;
using Microsoft.AspNetCore.Builder;

namespace SevenStar.Common.Extensions;

/// <summary>
/// Api 注入 Serilog 的擴充方法
/// </summary>
public static class SerilogHandlerExtension
{
    /// <summary>
    /// Http Api 專用，自動記錄每一筆 HTTP 請求的摘要日誌，包括方法、路徑、狀態碼與處理耗時，無需手動撰寫 log。
    /// </summary>
    /// <param name="app"></param>
    /// <param name="config">客製化的 Serilog 配置檔案，會調用 ConfigureRequestLogging 方法，可覆寫自行調整，傳入後會啟用</param>
    /// <returns></returns>
    public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder app, SerilogConfigurationBase? config = null)
    {
        config ??= new SerilogConfigurationBase();
        app.UseSerilogRequestLogging(config.ConfigureRequestLogging);

        return app;
    }
}