using RecordHub.OrderingService.Api;
using RecordHub.OrderingService.Api.Middlewares;
using RecordHub.OrderingService.Infrastructure.Data;
using RecordHub.OrderingService.Infrastructure.Extensions;
using RecordHub.Shared.Config;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddJwtAuth(builder.Configuration);
builder.Services.ConfigureSwagger();
builder.Services.AddCore(builder.Configuration);
builder.Services.AddPersistence(builder.Configuration);
builder.ConfigureSerilog();

var app = builder.Build();
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

await DbMigration.MigrateDatabase(app);

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
