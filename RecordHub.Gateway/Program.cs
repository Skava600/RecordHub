using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Services.AddOcelot(builder.Configuration);
builder.Services.AddSwaggerForOcelot(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddCors(options => options.AddPolicy("CorsPolicy", corsbuilder =>
{
    corsbuilder.AllowAnyMethod().AllowAnyHeader()
    .WithOrigins(builder.Configuration.GetValue<string>("ClientHost"))
    .AllowCredentials();
}));
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseRouting();
app.UseSwaggerForOcelotUI(opt =>
{
    opt.PathToSwaggerGenerator = app.Configuration["Ocelot:PathToSwaggerGen"];
});
app.UseCors("CorsPolicy");
await app.UseOcelot();

app.Run();
