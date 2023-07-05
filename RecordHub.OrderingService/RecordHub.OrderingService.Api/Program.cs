using Microsoft.EntityFrameworkCore;
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
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<OrderingDbContext>();
    var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();

    if (pendingMigrations.Any())
    {
        await dbContext.Database.MigrateAsync();
    }
}
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
