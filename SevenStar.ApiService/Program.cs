using SevenStar.Common.Api.Exception;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();

#region Swagger �t�m

// ���U Minimal API Explorer
builder.Services.AddEndpointsApiExplorer();
// ���U Swagger ���;�
builder.Services.AddSwaggerGen();

#endregion

// �Ȼs�ƨҥ~�B�z�ʧ@
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

// �M�ΰ򥻰��d�ˬd�Ϊ� http url: health & alive\
app.MapControllers();
app.MapDefaultEndpoints();

app.Run();