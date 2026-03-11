using System;
using System.Collections.Generic;
using System.Linq;
using Service.Models;
using Newtonsoft.Json;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;

namespace AlerterService
{
    public class AlertService : IAlerterService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public AlertService(IHttpClientFactory httpClientFactory, TelegramConfig config)
        {
            _httpClientFactory = httpClientFactory;
        }
         
        public async Task<bool> SendToTelegram(TelegramConfig config, string Content)
        {
            string uri = $"https://api.telegram.org/bot{config.Token}/sendMessage?chat_id={config.ChatId}&text={Content}";

            var json = JsonConvert.SerializeObject(new
            {
                chat_id = config.ChatId,
                text = Content
            });

            using (var client = _httpClientFactory.CreateClient())
            {
                HttpRequestMessage httpRequestMessage = new(HttpMethod.Post, uri)
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json"),
                };
                var response = await client.SendAsync(httpRequestMessage);

                return response.IsSuccessStatusCode;

            }
        }


    }
}