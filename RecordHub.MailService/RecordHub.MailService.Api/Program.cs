using Hangfire;
using RecordHub.MailService.Api.Extensions;
using RecordHub.MailService.Infrastructure;
using RecordHub.MailService.Infrastructure.Config;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

builder.Services.ConfigureMail(builder.Configuration);
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddMassTransit(builder.Configuration);
builder.ConfigureSerilog();

var app = builder.Build();
app.UseHangfireDashboard();
app.UseHttpsRedirection();
app.Run();
