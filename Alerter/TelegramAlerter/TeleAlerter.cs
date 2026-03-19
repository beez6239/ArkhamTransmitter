using System;
using System.Collections.Generic;
using System.Linq;
using AlerterService;
using System.Threading.Tasks;
using Service.Models;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;

namespace Alerter.TelegramAlerter
{

    public interface ITeleAlerter
    {
         public Task<bool> SendTelegram(string content);
    }

    public class TeleAlerterService : ITeleAlerter
    {
        private readonly IAlerterService _alertservice;
        private readonly ILogger<ITeleAlerter> _logger;
        private readonly TelegramConfig _options;
        public TeleAlerterService(IAlerterService alertservice, IOptions<TelegramConfig> options, ILogger<ITeleAlerter> logger)
        {
            _alertservice = alertservice;
            _options = options.Value;
            _logger = logger;
        }

        public async Task<bool> SendTelegram(string content)
        {
            try
            {
                var result = await _alertservice.SendToTelegram(_options, content);
                _logger.LogInformation(content);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to send message to telegram Actual Error: {ErrorMessage}", ex.ToString());
            }
            return false;

        }
    }

    public static class TeleAlerter
    {
        public static void RegisterTelegramAlerter(this WebApplication app)
        {
            app.MapPost("/AlertReceiver", async ([FromServices] ITeleAlerter alerter, HttpRequest request) =>
            {
                if (!request.Headers.TryGetValue("Arkham-Webhook-Token", out var A)) return Results.Unauthorized();
                using var sw = new StreamReader(request.Body);
                var content = await sw.ReadToEndAsync();
                if(string.IsNullOrWhiteSpace(content)) return Results.BadRequest();
                var result = await alerter.SendTelegram(content);
                return result ? Results.Ok() : Results.BadRequest();
            });

            app.MapGet("/Health", () =>
            {
                return Results.Ok("Alive");
            });
        }
    }
}