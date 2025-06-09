using Serilog;
using Common.Enums;
using Common.Api.Authentication;
using Common.Api.Exception;
using Common.Api.Localization;
using Common.Api.Swagger;
using SevenStar.Common.Extensions;
using SevenStar.Shared.Domain.Api.Auth.Jwt;
using SevenStar.Shared.Domain.Api.Logger.Serilog;
using SevenStar.Shared.Domain.Extensions;
using SevenStar.Shared.Domain.Service.Extensions;

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

    // 註冊動態查詢的 Provider 與 TokenService
    builder.Services.AddJwtTokenService();
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
    
    app.UseRouting();
    
    // app.UseAuthorizationHandling();
    // 加入 JWT Middleware（務必放在 UseAuthorization 之前）
    app.UseMiddleware<DynamicJwtAuthenticationMiddleware>();

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

