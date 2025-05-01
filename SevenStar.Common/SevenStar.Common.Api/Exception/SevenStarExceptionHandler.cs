using Common.Api.Exception;
using Common.Model.Exception;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

public class SevenStarExceptionHandler : ApiExceptionHandler
{
    public SevenStarExceptionHandler(ILogger<ApiExceptionHandler> logger, IProblemDetailsService problemDetailsService)
        : base(logger, problemDetailsService)
    {
    }

    protected override Dictionary<Type, ExceptionMetadata> GetExceptionMap()
    {
        var exceptionMap = base.GetExceptionMap();

        // 加入你自己專案額外的例外處理定義
        //map.Add(typeof(MyBusinessException), new ExceptionMetadata
        //{
        //    StatusCode = 422,
        //    ErrorCode = 2001,
        //    Title = "商業邏輯錯誤"
        //});

        //map.Add(typeof(MyServiceUnavailableException), new ExceptionMetadata
        //{
        //    StatusCode = 503,
        //    ErrorCode = 2002,
        //    Title = "服務暫時無法使用"
        //});

        return exceptionMap;
    }

    protected override ProblemDetails CreateProblemDetails(HttpContext httpContext, System.Exception exception, ExceptionMetadata exceptionMetadata)
    {
        var problem = base.CreateProblemDetails(httpContext, exception, exceptionMetadata);
        
        // 自訂擴充錯誤訊息
        // problem.Extensions.Add("errorCode", exceptionMetadata.ErrorCode);

        return problem;
    }
}
