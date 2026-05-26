using System;
using System.Collections.Generic;
using System.Linq;
using Service.Models;
using Newtonsoft.Json;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using AlerterService.Models;
using System.Text.Json.Serialization;
using System.Runtime.CompilerServices;

namespace AlerterService
{
    public class AlertService : IAlerterService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IUtility _helper;

        public AlertService(IHttpClientFactory httpClientFactory, IUtility helper)
        {
            _httpClientFactory = httpClientFactory;
            _helper = helper;
        }

        public async Task<bool> SendToTelegram(TelegramConfig config, string Content)
        {
               
               if(string.IsNullOrWhiteSpace(Content)) return false;

                var obj = _helper.ConvertContent(Content);
                if(obj == null) return false;
                string message = _helper.Formatmessage(obj);
               ArgumentNullException.ThrowIfNull(config);
               string uri = $"https://api.telegram.org/bot{config.Token}/sendMessage?chat_id={config.ChatId}";

                var json = JsonConvert.SerializeObject(new
                {
                    chat_id = config.ChatId,
                    text = message,
                    parse_mode = "HTML"
                });

                using var client = _httpClientFactory.CreateClient();
                HttpRequestMessage httpRequestMessage = new(HttpMethod.Post, uri)
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json"),
                };
                var response = await client.SendAsync(httpRequestMessage);

                return response.IsSuccessStatusCode;

        }

    }
}