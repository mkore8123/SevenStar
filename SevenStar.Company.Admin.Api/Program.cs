using Common.Api.Extensions;
using Common.Api.Localization;
using Common.Extensions;
using Serilog;
using SevenStar.Common.Api.Serilog;
using SevenStar.Common.Extensions;
using SevenStar.Data.Company.Npgsql.Extensions;
using System.Reflection;


var builder = WebApplication.CreateBuilder(args);

try
{
    var serilogConfig = new ApiSerilogConfiguration();
    // Add service defaults & Aspire client integrations.
    builder.AddServiceDefaults();
    builder.AddSerilogHandler(serilogConfig);
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
    app.UseExceptionHandling();
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

