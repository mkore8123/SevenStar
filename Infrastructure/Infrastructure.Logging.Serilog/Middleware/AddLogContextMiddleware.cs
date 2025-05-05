using Microsoft.AspNetCore.Http;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Logging.Serilog.Middleware;

/// <summary>
/// 針對 ILogger 紀錄添加更多 Metadata 欄位資訊
/// </summary>
public class AddLogContextMiddleware
{
    private readonly RequestDelegate _next;

    public AddLogContextMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        // 理論上目前不需要這段，因為當你使用 app.UseSerilogRequestLogging 預設就會注入 RequestId 等相關資訊
        using (LogContext.PushProperty("RequestId", context.TraceIdentifier))
        {
            await _next(context);
        }
    }
}
