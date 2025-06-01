using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.AspNetCore;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using System;
using System.Collections.Generic;
using System.Text;

namespace SevenStar.Shared.Domain.Api.Logger.Serilog;

public class ApiSerilogConfiguration : SerilogConfigurationBase
{
    public override LoggerConfiguration CreateLoggerConfiguration(IConfiguration? configuration = null)
    {
        var loggerConfig = base.CreateLoggerConfiguration();
        
        loggerConfig.WriteTo.Console(
            theme: AnsiConsoleTheme.Literate,
            outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}");

        return loggerConfig;
    }

    public override void ConfigureRequestLogging(RequestLoggingOptions options)
    {
        base.ConfigureRequestLogging(options);
    }
}