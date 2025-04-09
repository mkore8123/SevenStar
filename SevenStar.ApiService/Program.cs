using SevenStar.Common.Api.Exception;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();

#region Swagger 配置

// 註冊 Minimal API Explorer
builder.Services.AddEndpointsApiExplorer();
// 註冊 Swagger 產生器
builder.Services.AddSwaggerGen();

#endregion

// 客製化例外處理動作
builder.Services.AddExceptionHandler<CustomExceptionHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler ();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    //app.UseDeveloperExceptionPage();
    app.MapOpenApi();
}

app.UseRouting();

// 套用基本健康檢查用的 http url: health & alive\
app.MapControllers();
app.MapDefaultEndpoints();

app.Run();