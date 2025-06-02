using Common.Exception;
using Common.Exception.Model;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Common.Api.Exception;

/// <summary>
/// 全域例外處理器
/// </summary>
public class ApiExceptionHandler : BaseExceptionHandler, IExceptionHandler
{
    private readonly ILogger<ApiExceptionHandler> _logger;
    private readonly IProblemDetailsService _problemDetailsService;

    /// <summary>
    /// 建構函式
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="problemDetailsService"></param>
    public ApiExceptionHandler(ILogger<ApiExceptionHandler> logger, IProblemDetailsService problemDetailsService)
    {
        _logger = logger;
        _problemDetailsService = problemDetailsService;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, System.Exception exception, CancellationToken cancellationToken)
    {
        var metadata = Handle(exception, out var message);

        // 記錄日誌
        _logger.LogError(exception, "[{ErrorCode}] {ExceptionType}: {Message}, {StackTrace}", metadata.ErrorCode, exception.GetType().Name, exception.Message, exception.StackTrace);

        var problemDetails = CreateProblemDetails(httpContext, exception, metadata);

        await _problemDetailsService.WriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problemDetails,
            Exception = exception
        });

        return true;
    }

    /// <summary>
    /// 根據錯誤代碼及訊息配置 ProblemDetails 物件
    /// </summary>
    /// <param name="httpContext">http 請求</param>
    /// <param name="title">錯誤訊息標題</param>
    /// <param name="statusCode">錯誤訊息代碼</param>
    /// <param name="message">錯誤訊息內容</param>
    /// <param name="customErrorCode">客製化的錯誤代碼</param>
    /// <returns></returns>
    protected virtual ProblemDetails CreateProblemDetails(HttpContext httpContext, System.Exception exception, ExceptionMetadata exceptionMetadata)
    {
        var problem = new ProblemDetails
        {
            Title = exceptionMetadata.Title,
            Status = exceptionMetadata.StatusCode,
            Detail = exception.Message,
            Type = $"https://httpstatuses.com/{exceptionMetadata.StatusCode}",
            Instance = httpContext.Request.Path
        };

        // 加上額外欄位
        problem.Extensions.Add("errorCode", exceptionMetadata.ErrorCode);
        problem.Extensions.Add("RequestId", httpContext.TraceIdentifier);
        problem.Extensions.Add("timestamp", DateTime.UtcNow);

        return problem;
    }
}
