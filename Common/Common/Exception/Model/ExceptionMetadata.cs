using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Exception.Model;

public class ExceptionMetadata
{
    /// <summary>
    /// Http 狀態代碼
    /// </summary>
    public int StatusCode { get; init; }

    /// <summary>
    /// 系統客製化的錯誤代碼
    /// </summary>
    public int ErrorCode { get; init; }

    /// <summary>
    /// 錯誤訊息的標題
    /// </summary>
    public required string Title { get; init; } 
}
