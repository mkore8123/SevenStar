using Common.Extensions;
using Common.Api.Extensions;
using Common.Api.Localization;
using Serilog;
using SevenStar.Common.Api.Serilog;
using SevenStar.Common.Extensions;
using System.Reflection;


var builder = WebApplication.CreateBuilder(args);

try
{
    var serilogConfig = new ApiSerilogConfiguration();
    
    builder.AddSerilogHandler(serilogConfig);
    builder.Services.RegisterAssemblyHandling(Assembly.Load("SevenStar.Shared.Domain.Imp"));

    // Add service defaults & Aspire client integrations.
    // builder.AddServiceDefaults();

    // Add services to the container.
    builder.Services.AddLocalizationHandler(new SevenStarLocalization());

    // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
    builder.Services.AddOpenApi();
    builder.Services.AddControllers();

    #region Swagger 配置

    // 註冊 Minimal API Explorer
    builder.Services.AddEndpointsApiExplorer();
    // 註冊 Swagger 產生器
    builder.Services.AddSwaggerGenHandler();

    #endregion

    // 客製化例外處理動作
    builder.Services.AddExceptionHandler();

    var app = builder.Build();
    app.UseLocalizationHandling();

    // app.UseMiddleware<AddLogContextMiddleware>();
    app.UseExceptionHandling(serilogConfig);

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
    }

    app.UseSwaggerUIHandling();
    app.UseExceptionHandling();
    app.UseRouting();
    
    // 套用基本健康檢查用的 http url: health & alive
    app.MapControllers();
    // app.MapDefaultEndpoints();

    app.Run();
}
catch (Exception except)
{
    Log.Fatal(except, "Application start-up failed.");
}
finally
{
    Log.CloseAndFlush();
}

