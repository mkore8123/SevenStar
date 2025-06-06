﻿using Common.Api.Authentication;
using Common.Api.Exception;
using Common.Api.Localization;
using Common.Api.Swagger;
using Common.Enums;
using Serilog;
using SevenStar.Common.Extensions;
using SevenStar.Shared.Domain.Api.Logger.Serilog;
using SevenStar.Shared.Domain.Extensions;
using SevenStar.Shared.Domain.Imp.Service;
using SevenStar.Shared.Domain.Service;
using SevenStar.Shared.Domain.Service.Extensions;
using System.Reflection;

var companyId = 1;
var platformDbConnectionString = "Host=127.0.0.1;Port=5432;Username=postgres;Password=apeter56789;Database=postgres;SearchPath=public;";
var builder = WebApplication.CreateBuilder(args);

try
{
    var serilogConfig = new ApiSerilogConfiguration();
    
    // Add service defaults & Aspire client integrations.
    builder.AddServiceDefaults();
    builder.AddSerilogHandler(serilogConfig);
    
    builder.Services.AddPlatformDb(DataSource.PostgreSql, platformDbConnectionString);
    builder.Services.AddCompanyGamesDb(companyId);
    builder.Services.AddCompanyRedisDb(companyId);
    builder.Services.AddDomainKeyedServices();

    // builder.Services.AddCompanyJwtOption(companyId);

    builder.Services.AddControllers(); 
    builder.Services.AddSwaggerGenHandler();
    builder.Services.AddExceptionHandler(); // 客製化例外處理動作
    builder.Services.AddLocalizationHandler(new SevenStarLocalization());

    var app = builder.Build();
    app.UseLocalizationHandling();

    // app.UseMiddleware<AddLogContextMiddleware>();
    app.UseExceptionHandling(serilogConfig);

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
    }

    app.UseSwaggerUIHandling();
    // app.UseAuthorizationHandling();
    app.UseRouting();
    
    // 套用基本健康檢查用的 http url: health & alive
    app.MapControllers();
    app.MapDefaultEndpoints();

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

