using RecordHub.MailService.Api.Extensions;
using RecordHub.MailService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);


builder.Services.ConfigureMail(builder.Configuration);
builder.Services.AddInfrastructureServices();
builder.Services.AddMassTransit();
var app = builder.Build();

app.UseHttpsRedirection();

app.Run();
