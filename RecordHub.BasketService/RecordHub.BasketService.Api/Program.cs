using RecordHub.BasketService.Api;
using RecordHub.BasketService.Api.Middlewares;
using RecordHub.BasketService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.ConfigureSwagger();
builder.Services.AddRedisPersistence(builder.Configuration);
builder.Services.AddJwtAuth(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
if (!app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
