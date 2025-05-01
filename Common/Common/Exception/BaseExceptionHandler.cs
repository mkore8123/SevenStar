using Common.Model.Exception;
using System.ComponentModel.DataAnnotations;
using System.Security;
using System.Security.Authentication;

namespace Common.Exception;

/// <summary>
/// 基礎例外處理類別
/// </summary>
public abstract class BaseExceptionHandler
{
    /// <summary>
    /// 取得目前的例外對應表（可被子類別擴充）
    /// </summary>
    protected virtual Dictionary<Type, ExceptionMetadata> GetExceptionMap()
    {
        return new Dictionary<Type, ExceptionMetadata>
        {
            { typeof(ValidationException), new ExceptionMetadata { StatusCode = 400, ErrorCode = 1001, Title = "驗證失敗" } },
            { typeof(ArgumentException), new ExceptionMetadata { StatusCode = 400, ErrorCode = 1002, Title = "無效參數" } },
            { typeof(ArgumentNullException), new ExceptionMetadata { StatusCode = 400, ErrorCode = 1003, Title = "必要參數遺漏" } },
            { typeof(ArgumentOutOfRangeException), new ExceptionMetadata { StatusCode = 400, ErrorCode = 1004, Title = "參數超出範圍" } },
            { typeof(FormatException), new ExceptionMetadata { StatusCode = 400, ErrorCode = 1005, Title = "格式錯誤" } },
            { typeof(InvalidOperationException), new ExceptionMetadata { StatusCode = 409, ErrorCode = 1006, Title = "無效操作" } },
            { typeof(UnauthorizedAccessException), new ExceptionMetadata { StatusCode = 401, ErrorCode = 1007, Title = "未授權" } },
            { typeof(SecurityException), new ExceptionMetadata { StatusCode = 403, ErrorCode = 1008, Title = "權限不足" } },
            { typeof(AuthenticationException), new ExceptionMetadata { StatusCode = 401, ErrorCode = 1009, Title = "身份驗證失敗" } },
            { typeof(KeyNotFoundException), new ExceptionMetadata { StatusCode = 404, ErrorCode = 1010, Title = "資源未找到" } },
            { typeof(FileNotFoundException), new ExceptionMetadata { StatusCode = 404, ErrorCode = 1011, Title = "檔案不存在" } },
            { typeof(DirectoryNotFoundException), new ExceptionMetadata { StatusCode = 404, ErrorCode = 1012, Title = "目錄不存在" } },
            { typeof(NotImplementedException), new ExceptionMetadata { StatusCode = 501, ErrorCode = 1013, Title = "尚未實作" } },
            { typeof(NotSupportedException), new ExceptionMetadata { StatusCode = 405, ErrorCode = 1014, Title = "不支援的操作" } },
            { typeof(TimeoutException), new ExceptionMetadata { StatusCode = 504, ErrorCode = 1015, Title = "操作逾時" } },
            { typeof(OperationCanceledException), new ExceptionMetadata { StatusCode = 499, ErrorCode = 1016, Title = "用戶取消操作" } },
            { typeof(IOException), new ExceptionMetadata { StatusCode = 500, ErrorCode = 1019, Title = "I/O 錯誤" } },
            { typeof(ObjectDisposedException), new ExceptionMetadata { StatusCode = 500, ErrorCode = 1020, Title = "資源已被釋放" } },
            { typeof(StackOverflowException), new ExceptionMetadata { StatusCode = 500, ErrorCode = 1021, Title = "堆疊溢位" } },
            { typeof(OutOfMemoryException), new ExceptionMetadata { StatusCode = 500, ErrorCode = 1022, Title = "記憶體不足" } },
            { typeof(TaskCanceledException), new ExceptionMetadata { StatusCode = 504, ErrorCode = 1023, Title = "非同步作業取消" } },
            { typeof(HttpRequestException), new ExceptionMetadata { StatusCode = 502, ErrorCode = 1024, Title = "HTTP 請求錯誤" } },
        };
    }

    /// <summary>
    /// 將例外訊息轉換成中繼例外物件
    /// </summary>
    /// <param name="exception"></param>
    /// <returns></returns>
    protected virtual ExceptionMetadata GetMetadata(System.Exception exception)
    {
        var type = exception.GetType();
        var map = GetExceptionMap();

        while (type != typeof(object))
        {
            if (map.TryGetValue(type, out var metadata))
                return metadata;

            type = type.BaseType!;
        }

        return new ExceptionMetadata
        {
            StatusCode = 500,
            ErrorCode = 1000,
            Title = "伺服器錯誤"
        };
    }

    public virtual ExceptionMetadata Handle(System.Exception exception, out string message)
    {
        var metadata = GetMetadata(exception);
        message = exception.Message;

        return metadata;
    }
}

