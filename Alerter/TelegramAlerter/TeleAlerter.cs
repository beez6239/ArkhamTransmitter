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
        private readonly TelegramConfig _options;
        public TeleAlerterService(IAlerterService alertservice, IOptions<TelegramConfig> options)
        {
            _alertservice = alertservice;
            _options = options.Value;
        }

        public async Task<bool> SendTelegram(string content)
        {
           var result =  await _alertservice.SendToTelegram(_options, content);
            return false;
        }
    }

    public static class TeleAlerter
    {
        public static void RegisterTelegramAlerter(this WebApplication app)
        {
            app.MapGet("/AlertReceiver", async ([FromServices] ITeleAlerter alerter, string content) =>
            {
                var result = await alerter.SendTelegram(content);
                if (result)
                {
                    return Results.Json("sent");
                }
                return Results.BadRequest();
            });
        }
    }
}