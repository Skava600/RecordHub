using RecordHub.ChatService.Api;
using RecordHub.ChatService.Api.Extensions;
using RecordHub.ChatService.Api.Hubs;
using RecordHub.ChatService.Api.Middlewares;
using RecordHub.ChatService.Infrastructure.Extensions;
using RecordHub.Shared.Extensions;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.ConfigureSwagger();
builder.Services.AddSignalR();
builder.Services.AddClientCors(builder.Configuration);
builder.Services.AddConfiguredJwtBearer(builder.Configuration);
builder.Services.AddAuthorization();
builder.Services.AddSingleton<IDictionary<string, UserConnection>>(opts => new Dictionary<string, UserConnection>());
builder.Services.AddMongoDbPersistence(builder.Configuration);
builder.Services.AddInfrastructureServices();

var app = builder.Build();

app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseCors("CorsPolicy");
app.MapControllers();
app.MapHub<ChatHub>("/chat");
app.Run();
