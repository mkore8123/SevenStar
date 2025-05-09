﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Common.Api.Extensions;

/// <summary>
/// Web Api 自訂啟用 Swagger 流程
/// </summary>
public static class SwaggerGenExtension
{
    /// <summary>
    /// 註冊 Swagger 相關配置
    /// </summary>
    public static IServiceCollection AddSwaggerGenHandler(this IServiceCollection services)
    {
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