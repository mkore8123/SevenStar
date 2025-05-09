using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.AspNetCore;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace SevenStar.Common.Api.Serilog;

public class ApiSerilogConfiguration : SerilogConfigurationBase
{
    public override LoggerConfiguration CreateLoggerConfiguration(IConfiguration? configuration = null)
    {
        var loggerConfig = base.CreateLoggerConfiguration();

        return loggerConfig;
    }

    public override void ConfigureRequestLogging(RequestLoggingOptions options)
    {
        base.ConfigureRequestLogging(options);
    }
}