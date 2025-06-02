using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Common.Api.Swagger;

/// <summary>
/// Web Api 自訂啟用 Swagger 流程
/// </summary>
public static class SwaggerGenExtension
{
    /// <summary>
    /// 註冊 Swagger 相關配置
    /// </summary>
    public static IServiceCollection AddSwaggerGenHandler(this IServiceCollection services, bool sSupportMinimalApi = false)
    {
        if (sSupportMinimalApi)
        {
            services.AddOpenApi(); // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            services.AddEndpointsApiExplorer();  // 註冊 Minimal API Explorer，通常 AddOpenApi 就已經包含註冊
        }
        
        services.AddSwaggerGen();

        return services;
    }

    /// <summary>
    /// 中介層使用 Swagger
    /// </summary>
    public static IApplicationBuilder UseSwaggerUIHandling(this IApplicationBuilder app)
    {
        var env = app.ApplicationServices.GetRequiredService<IWebHostEnvironment>();

        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        else {}

        return app;
    }
}