﻿using Serilog;
using Microsoft.AspNetCore.Builder;

namespace SevenStar.Common.Extensions;

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

        builder.Host.UseSerilog();
        builder.Logging.AddSerilog(Log.Logger);  //需要詳加研究加上這一段的差別

        return builder;
    }
}