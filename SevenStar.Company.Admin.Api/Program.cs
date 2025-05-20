using Common.Api.Extensions;
using Common.Api.Localization;
using Common.Api.Option;
using Common.Api.Token.Jwt;
using Common.Extensions;
using Infrastructure.Caching.Redis;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using SevenStar.Common.Api.Serilog;
using SevenStar.Common.Extensions;
using SevenStar.Data.Company.Npgsql.Extensions;
using SevenStar.Shared.Domain.Api.Extensions;
using SevenStar.Shared.Domain.Api.Token;
using SevenStar.Shared.Domain.Extensions;
using StackExchange.Redis;
using System.Reflection;

var companyId = 1;
var builder = WebApplication.CreateBuilder(args);

try
{
    var serilogConfig = new ApiSerilogConfiguration();
    // Add service defaults & Aspire client integrations.
    builder.AddServiceDefaults();
    builder.AddSerilogHandler(serilogConfig);

    #region jwt

    
    builder.Services.AddJwtOptionProvider(companyId);
    builder.Services.AddCompanyRedisDatabases(companyId);
    
    builder.Services.AddAuthorization();
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();

    // 使用 PostConfigure 來設定 JwtBearerOptions
    builder.Services.PostConfigure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        // 注意：這裡才有 ServiceProvider 可用於注入服務
        using var provider = builder.Services.BuildServiceProvider();
        var tokenService = provider.GetRequiredService<JwtTokenService>();

        options.Events = tokenService.CreateJwtBearerEvents();
        options.TokenValidationParameters = tokenService.CreateValidationParameters();
    });


    #endregion


    builder.Services.AddHybridCacheHandler("localhost:6379,defaultDatabase=0,allowAdmin=true,connectTimeout=5000,abortConnect=false");
    builder.Services.AddCompanyDbHandler(1, "Host=127.0.0.1;Port=5432;Username=postgres;Password=apeter56789;Database=postgres;SearchPath=public;");
    builder.Services.RegisterAssemblyHandling(Assembly.Load("SevenStar.Shared.Domain.Imp"));

    builder.Services.AddLocalizationHandler(new SevenStarLocalization());

    builder.Services.AddControllers(); 
    builder.Services.AddSwaggerGenHandler();

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
    app.UseAuthorizationHandling();
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

