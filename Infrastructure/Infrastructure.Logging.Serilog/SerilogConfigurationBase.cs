using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.AspNetCore;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Sinks.SystemConsole.Themes;
using System.Reflection;

/// <summary>
/// Serilog 的共用設定基底類別，可讓 ASP.NET API 與 Worker 專案共用統一的 Logger 與 RequestLog 設定方式。
/// 支援透過繼承方式覆寫 CreateLoggerConfiguration 或 ConfigureRequestLogging 實現客製化。
/// </summary>
public class SerilogConfigurationBase
{
    /// <summary>
    /// 建立全域 LoggerConfiguration 設定，可由 appsettings.json 或程式碼方式建立。
    /// 支援最低層級、自訂欄位、例外詳情、輸出管道等。
    /// </summary>
    /// <param name="configuration">可選的組態來源，例如從 appsettings.json 建立</param>
    /// <returns>建構完成的 LoggerConfiguration</returns>
    public virtual LoggerConfiguration CreateLoggerConfiguration(IConfiguration? configuration = null)
    {
        var loggerConfig = new LoggerConfiguration();

        // 若提供 IConfiguration，優先從設定檔建立 logger
        if (configuration != null)
        {
            return loggerConfig.ReadFrom.Configuration(configuration);
        }

        return loggerConfig
            // 1️⃣ 基本層級設定
            // .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("System", LogEventLevel.Warning)

            // 2️⃣ 加入常用的結構化欄位
            .Enrich.WithMachineName() // 寫入目前主機名稱，可用於分散式系統或容器中識別來源主機
            .Enrich.WithThreadId()    // 寫入目前執行緒編號，有助於除錯多執行緒應用程式問題
            .Enrich.WithExceptionDetails() // 將例外的 StackTrace、InnerException、Data 等詳細內容序列化為結構化屬性（需 Serilog.Exceptions 套件）
            .Enrich.WithEnvironmentName()  // 寫入 ASPNETCORE_ENVIRONMENT 變數（如 Development, Production）
            .Enrich.WithProperty("AppName", Assembly.GetEntryAssembly()?.GetName().Name ?? "UnknownApp") // 寫入自訂欄位 AppName，預設為應用程式組件名稱（如 SevenStar.Api）
            .Enrich.FromLogContext();  // 啟用從 LogContext（例如 middleware 或 filter）動態推送的欄位，如 UserId、JobId

            // ✅ 若有需要，也可以啟用以下 Enricher：
            // .Enrich.WithProcessId()
            // .Enrich.WithProcessName()

            // 3️⃣ 設定輸出目標 - Console（可擴充為 File、Seq、Elasticsearch 等）
            //.WriteTo.Console(
            //    theme: AnsiConsoleTheme.Literate,
            //    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}");
    }

    /// <summary>
    /// 設定 ASP.NET Web API 專案的 HTTP 請求記錄中介層，會自動為每筆請求產生摘要 log。
    /// 包含 HTTP 方法、路徑、狀態碼、耗時，以及其他診斷欄位（如 IP、UserAgent）。
    /// </summary>
    /// <param name="options">RequestLoggingOptions 實例，可供覆寫預設行為</param>
    public virtual void ConfigureRequestLogging(RequestLoggingOptions options)
    {
        // 設定請求記錄的訊息模板，用來輸出請求摘要
        options.MessageTemplate =
            "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";

        // 根據 HTTP 狀態碼與例外錯誤，動態決定 log 層級
        options.GetLevel = (ctx, elapsed, ex) =>
            ex != null || ctx.Response.StatusCode >= 500
                ? LogEventLevel.Error // 若有例外或狀態碼 >= 500，視為錯誤
                : LogEventLevel.Information; // 否則為資訊等級

        // 設定額外需寫入 DiagnosticContext 的欄位，這些欄位會進入 log 中
        options.EnrichDiagnosticContext = (diag, ctx) =>
        {
            var headers = ctx.Request.Headers; // 取得 HTTP 請求標頭

            diag.Set("UserAgent", headers["User-Agent"].ToString()); // 記錄瀏覽器或裝置類型
            diag.Set("RemoteIp", ctx.Connection.RemoteIpAddress?.ToString() ?? string.Empty); // 原始用戶端 IP 位址
            diag.Set("Referer", headers["Referer"].ToString()); // 記錄來源頁面（若有）
            diag.Set("ForwardedFor", headers["X-Forwarded-For"].ToString()); // 記錄反向代理來源 IP（如 Nginx）
            diag.Set("Host", ctx.Request.Host.Value ?? string.Empty); // 請求主機（含 Port）
            diag.Set("Scheme", ctx.Request.Scheme); // HTTP or HTTPS
            diag.Set("RequestId", ctx.TraceIdentifier); // 追蹤 ID（內建於 ASP.NET Core）
            diag.Set("RequestContentType", ctx.Request.ContentType ?? string.Empty); // 請求的內容類型（如 application/json）
            diag.Set("RequestProtocol", ctx.Request.Protocol); // 協定版本（如 HTTP/1.1）
        };
    }
}
