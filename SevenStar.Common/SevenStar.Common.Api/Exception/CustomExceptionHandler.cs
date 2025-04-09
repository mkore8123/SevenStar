using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace SevenStar.Common.Api.Exception;

public class CustomExceptionHandler : IExceptionHandler
{
    private readonly ILogger<CustomExceptionHandler> logger;
    public CustomExceptionHandler(ILogger<CustomExceptionHandler> logger)
    {
        this.logger = logger;
    }

    /// <summary>
    /// 客製化例外處理
    /// 如果啟用 app.UseDeveloperExceptionPage(); 則出現例外不會進入此方法
    /// </summary>
    /// <param name="httpContext"></param>
    /// <param name="exception"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public ValueTask<bool> TryHandleAsync(HttpContext httpContext, System.Exception exception, CancellationToken cancellationToken)
    {
        var exceptionMessage = exception.Message;
        logger.LogError(
            "Error Message: {exceptionMessage}, Time of occurrence {time}",
            exceptionMessage, DateTime.UtcNow);
        // Return false to continue with the default behavior
        // - or - return true to signal that this exception is handled
        return ValueTask.FromResult(false);
    }
}
