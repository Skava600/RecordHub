using RecordHub.BasketService.Api;
using RecordHub.BasketService.Api.Middlewares;
using RecordHub.BasketService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.ConfigureSwagger();
builder.Services.AddRedisPersistence(builder.Configuration);
builder.Services.AddJwtAuth(builder.Configuration);
builder.Services
    .AddGrpcClient<CatalogChecker.CatalogCheckerClient>(o =>
    {
        o.Address = new Uri("https://recordhub.catalogservice.grpc");
        o.ChannelOptionsActions.Clear();
        o.ChannelOptionsActions.Add((opt) =>
            {
                opt.UnsafeUseInsecureChannelCallCredentials = true;
            });
    })
    .ConfigurePrimaryHttpMessageHandler(() =>
    {
        var handler = new HttpClientHandler();
        handler.ServerCertificateCustomValidationCallback =
            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
        return handler;
    });
builder.Services.AddCore(builder.Configuration);
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
