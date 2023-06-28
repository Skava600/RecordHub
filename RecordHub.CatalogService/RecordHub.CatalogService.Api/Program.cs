using RecordHub.CatalogService.Api;
using RecordHub.CatalogService.Api.Middlewares;
using RecordHub.CatalogService.Infrastructure;
using RecordHub.CatalogService.Infrastructure.Config;
using RecordHub.CatalogService.Infrastructure.Extensions;
using RecordHub.CatalogService.Infrastructure.Services;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc(opts =>
{
    opts.EnableDetailedErrors = true;
});
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.ConfigureSwagger();
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddInfrastructure();
builder.Services.AddJwtAuth(builder.Configuration);
builder.ConfigureSerilog();
var app = builder.Build();
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline.
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

}
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapGrpcService<CatalogCheckerService>();
app.MapControllers();

app.Run();
