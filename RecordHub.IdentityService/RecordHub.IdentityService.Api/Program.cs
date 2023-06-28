using RecordHub.IdentityService.Api;
using RecordHub.IdentityService.Api.Middlewares;
using RecordHub.IdentityService.Infrastructure;
using RecordHub.IdentityService.Infrastructure.Configuration;
using RecordHub.IdentityService.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.ConfigureSwagger();

builder.ConfigureSerilog();
builder.Services.AddCore(builder.Configuration);
builder.Services.AddPersistence(builder.Configuration);
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

    var dbInitializer = services.GetRequiredService<DbInitializer>();

    await dbInitializer.Initialize();

}
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
