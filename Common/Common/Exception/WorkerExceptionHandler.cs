using Common.Model.Exception;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Exception;

public class WorkerExceptionHandler : BaseExceptionHandler
{
    private readonly ILogger<WorkerExceptionHandler> _logger;

    public WorkerExceptionHandler(ILogger<WorkerExceptionHandler> logger)
    {
        _logger = logger;
    }

    protected override Dictionary<Type, ExceptionMetadata> GetExceptionMap() 
    {
        var exceptionMap = base.GetExceptionMap();
        exceptionMap.Add(typeof(WorkerException), new ExceptionMetadata { StatusCode = 0, ErrorCode = 1001, Title = "排程錯誤例外" });

        return exceptionMap;
    }


    public void HandleWorkerException(System.Exception ex)
    {
        var metadata = Handle(ex, out var message);

        _logger.LogError(ex, "[Worker Error][{Code}] {Type}: {Title} - {Message}",
            metadata.ErrorCode, ex.GetType().Name, metadata.Title, message);

        // 可擴充：寄信、寫檔、報警等等
    }
}
