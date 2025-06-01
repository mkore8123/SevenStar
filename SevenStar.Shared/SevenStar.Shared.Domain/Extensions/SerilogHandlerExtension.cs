using Serilog;
using Microsoft.AspNetCore.Builder;

namespace SevenStar.Shared.Domain.Extensions;

/// <summary>
/// 注入 Serilog 的擴充方法
/// </summary>
public static class SerilogHandlerExtension
{
    /// <summary>
    /// 註冊配置 Serilog 的設置項目，並將其設置為全域的日誌記錄器。
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="config">客製化的 Serilog 配置檔案，會調用 CreateLoggerConfiguration 方法，可覆寫自行調整，傳入後會啟用</param>
    /// <returns></returns>
    public static WebApplicationBuilder AddSerilogHandler(this WebApplicationBuilder builder, SerilogConfigurationBase? config = null)
    {
        config ??= new SerilogConfigurationBase();
        Log.Logger = config.CreateLoggerConfiguration(builder.Configuration).CreateLogger();

        // 告訴 ASP.NET Core Host 使用你剛剛設定的 Log.Logger 作為應用程式主記錄器。
        builder.Host.UseSerilog();

        // 替代 ILogger<T> 系統，讓應用內部使用 Serilog 作為主要的 log 提供者
        builder.Logging.AddSerilog(Log.Logger); 

        return builder;
    }
}