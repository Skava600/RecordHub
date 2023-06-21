using RecordHub.MailService.Api.Extensions;
using RecordHub.MailService.Infrastructure;
using RecordHub.MailService.Infrastructure.Config;

var builder = WebApplication.CreateBuilder(args);


builder.Services.ConfigureMail(builder.Configuration);
builder.Services.AddInfrastructureServices();
builder.Services.AddMassTransit();
builder.ConfigureSerilog();
var app = builder.Build();

app.UseHttpsRedirection();

app.Run();
