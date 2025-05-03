using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.AspNetCore;
using Serilog.Events;
using Serilog.Exceptions; // 提供更詳細的例外結構化資訊
using Serilog.Sinks.SystemConsole.Themes;
using System.Reflection;

/// <summary>
/// Serilog 的共用設定基底類別，提供 Logger 建立與 RequestLogging 設定，支援 API 與 Worker 共用。
/// 可透過繼承方式覆寫設定內容達到客製化目的。
/// </summary>
public class SerilogConfigurationBase
{
    /// <summary>
    /// 配置 Worker/Api 全域 Serilog LoggerConfiguration，可在子類中覆寫。
    /// 包含最低層級、常用 enrichers、錯誤細節與 fallback 設定。
    /// </summary>
    public virtual LoggerConfiguration CreateLoggerConfiguration(IConfiguration? configuration = null)
    {
        var loggerConfig = new LoggerConfiguration();

        // 若傳入 IConfiguration，優先採用設定檔（如 appsettings.json）的配置
        if (configuration != null)
        {
            return loggerConfig.ReadFrom.Configuration(configuration);
        }

        loggerConfig
            // 設定全域最小日誌等級為 Debug，可用來觀察開發與測試行為
            .MinimumLevel.Debug()

            // 降低來自 Microsoft 與 System 命名空間的噪音（預設為 Warning）
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("System", LogEventLevel.Warning)

            // 將當前機器名稱寫入 log，例如在容器或分散式環境中有助於辨識來源
            .Enrich.WithMachineName()

            // 將執行緒 ID 寫入 log，有助於除錯並觀察執行緒切換情況
            .Enrich.WithThreadId()

            // 寫入目前 Process ID，可協助追蹤應用執行序
            // .Enrich.WithProcessId()

            // 寫入執行中程式名稱，例如 dotnet、MyApp.Api 等
            // .Enrich.WithProcessName()

            // 將例外堆疊與屬性結構化寫入 log（需 Serilog.Exceptions 套件）
            .Enrich.WithExceptionDetails()

            // 設定自訂欄位 AppName，讀取目前組件名稱，例如 SevenStar.Api
            .Enrich.WithProperty("AppName", Assembly.GetEntryAssembly()?.GetName().Name ?? "UnknownApp")

            // 寫入 ASPNETCORE_ENVIRONMENT 變數，如 Development、Production
            .Enrich.WithEnvironmentName()

            // 開啟從 LogContext 推入的所有欄位（例如 UserId、JobId）會自動附加到 log
            .Enrich.FromLogContext()

            // ✅ 加入這一行：將 log 寫出到主控台
            .WriteTo.Console(theme: AnsiConsoleTheme.Literate,
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}");

        return loggerConfig;
    }

    /// <summary>
    /// 僅適用 Web Api 站點專案，自動為每筆 Http 請求做第一筆 Log 紀錄：
    /// - 自動記錄每筆 HTTP 請求的摘要資訊（方法、路徑、狀態碼、耗時）
    /// - 設定 log 等級（Error/Information）
    /// - 擴充 log 結構欄位（如 UserAgent、IP 等），便於在 ELK / Seq 等工具查詢
    /// </summary>
    public virtual void ConfigureRequestLogging(RequestLoggingOptions options)
    {
        // 設定輸出訊息模板，預設包含 HTTP 方法、路徑、狀態碼與耗時
        options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";

        // 根據是否例外或 HTTP 狀態碼決定日誌等級
        options.GetLevel = (ctx, elapsed, ex) =>
            ex != null || ctx.Response.StatusCode >= 500
                ? LogEventLevel.Error
                : LogEventLevel.Information;

        // 注入額外欄位到 DiagnosticContext 中，這些欄位可結構化輸出到 JSON log 並支援分析
        options.EnrichDiagnosticContext = (diag, ctx) =>
        {
            var headers = ctx.Request.Headers;

            diag.Set("UserAgent", headers["User-Agent"].ToString());    // 紀錄 User-Agent 標頭，可用於分析裝置、瀏覽器類型
            diag.Set("RemoteIp", ctx.Connection.RemoteIpAddress?.ToString() ?? string.Empty); // 紀錄原始用戶 IP 位址
            diag.Set("Referer", headers["Referer"].ToString());  // Referer 可觀察請求來源（如從哪個頁面跳轉而來）
            diag.Set("ForwardedFor", headers["X-Forwarded-For"].ToString()); // 在反向代理中紀錄原始 IP（如 Nginx, Cloudflare）
            diag.Set("Host", ctx.Request.Host.Value ?? string.Empty); // 記錄請求的 Host 名稱
            diag.Set("Scheme", ctx.Request.Scheme); // HTTP 或 HTTPS 協定
            diag.Set("RequestId", ctx.TraceIdentifier); // ASP.NET Core 的追蹤 ID，可追蹤單一請求生命週期
            diag.Set("RequestContentType", ctx.Request.ContentType ?? string.Empty);  // 請求的 Content-Type，例如 application/json
            diag.Set("RequestProtocol", ctx.Request.Protocol); // 請求的協定版本，例如 HTTP/1.1
        };
    }
}
