using RecordHub.CatalogService.Api;
using RecordHub.CatalogService.Api.GrpcServices;
using RecordHub.CatalogService.Api.Middlewares;
using RecordHub.CatalogService.Infrastructure;
using RecordHub.CatalogService.Infrastructure.Elasticsearch;
using RecordHub.CatalogService.Infrastructure.Extensions;
using RecordHub.Shared.Config;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc(opts =>
{
    opts.EnableDetailedErrors = true;
});
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.ConfigureSwagger();
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddInfrastructure();
builder.Services.AddElasticsearch(builder.Configuration);
builder.Services.AddJwtAuth(builder.Configuration);
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
    var context = services.GetRequiredService<ApplicationDbContext>();
    context.Database.EnsureCreated();
    var esInit = services.GetRequiredService<IndexInitializer>();
    await esInit.Initialize();
}
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapGrpcService<CatalogCheckerService>();
app.MapControllers();

app.Run();

public partial class Program { }