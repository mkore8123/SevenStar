using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace SevenStar.Common.Extensions;

/// <summary>
/// Worker 注入 Serilog 的擴充方法
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

        builder.Host.UseSerilog();

        return builder;
    }
}