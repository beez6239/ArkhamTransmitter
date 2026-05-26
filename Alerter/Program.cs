using AlerterService;
using Service.Models;
using Alerter.TelegramAlerter;
using Alerter.Utility;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAlerterService, AlertService>();
builder.Services.AddScoped<ITeleAlerter, TeleAlerterService>();
builder.Services.AddScoped<TelegramConfig>();
builder.Services.AddScoped<IUtility, Utiliy>();
builder.Services.AddHttpClient();

builder.Services.Configure<TelegramConfig>(builder.Configuration.GetSection("TelegramConfig"));
var app = builder.Build();


app.RegisterTelegramAlerter();
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";

app.Run($"http://0.0.0.0:{port}");
