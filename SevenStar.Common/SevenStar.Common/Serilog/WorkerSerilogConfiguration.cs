using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.AspNetCore;
using Serilog.Events;
using Serilog.Sinks.Grafana.Loki;
using System;
using System.Collections.Generic;
using System.Text;

namespace SevenStar.Common.Serilog;

public class WorkerSerilogConfiguration : SerilogConfigurationBase
{
    /// <summary>
    /// 自訂 Worker 的 Serilog 設定
    /// </summary>
    public override LoggerConfiguration CreateLoggerConfiguration(IConfiguration? configuration = null)
    {
        var loggerConfig = base.CreateLoggerConfiguration();
        // loggerConfig.WriteTo.GrafanaLoki("http://localhost:3100");

        return loggerConfig;
    }
}
