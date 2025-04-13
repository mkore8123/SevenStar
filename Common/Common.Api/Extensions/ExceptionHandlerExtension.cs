using Common.Api.Exception;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Common.Api.Extensions;

/// <summary>
/// Web Api 自訂例外處理流程
/// </summary>
public static class ExceptionHandlerExtension
{
    /// <summary>
    /// 註冊例外處理所需服務
    /// </summary>
    public static IServiceCollection AddExceptionHandling(this IServiceCollection services)
    {
        services.AddProblemDetails();
        services.AddExceptionHandler<CustomExceptionHandler>();

        return services;
    }

    /// <summary>
    /// 使用例外處理中介層
    /// </summary>
    public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder app)
    {
        var env = app.ApplicationServices.GetRequiredService<IWebHostEnvironment>();

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            /*
             * 正常來說，有註冊 builder.Services.AddExceptionHandler 要配合 app.UseExceptionHandler 才能正常使用。
             * 但在 ASP.NET Core 8 起會自動檢查是否有註冊 IExceptionHandler，如果有，就自動注入一個對應的 Middleware 調用 builder.Services.AddExceptionHandler。
             * 
             * 因此是否有用 app.UseExceptionHandler(); 已非必要。
             * 但為了這樣可讀性更高，行為也更明確，避免「看不見的魔法」混淆他人（或日後的你自己 😆），仍採取明確調用。
             */
            app.UseExceptionHandler();
        }

        return app;
    }
}