using AlerterService;
using Service.Models;
using Alerter.TelegramAlerter; 

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAlerterService, AlertService>();
builder.Services.AddScoped<ITeleAlerter, TeleAlerterService>();
builder.Services.AddScoped<TelegramConfig>();
builder.Services.AddHttpClient();

builder.Services.Configure<TelegramConfig>(builder.Configuration.GetSection("TelegramConfig"));
var app = builder.Build();


app.RegisterTelegramAlerter();
app.Run();
