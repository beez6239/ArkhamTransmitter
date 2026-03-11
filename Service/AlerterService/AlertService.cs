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

        public AlertService(IHttpClientFactory httpClientFactory, TelegramConfig config)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<bool> SendToTelegram(TelegramConfig config, string Content)
        {
            var obj = ConvertContent(Content);
            string message = string.Empty;
            if (obj != null)
            {
                message = Formatmessage(obj);
            }

            string uri = $"https://api.telegram.org/bot{config.Token}/sendMessage?chat_id={config.ChatId}";

            var json = JsonConvert.SerializeObject(new
            {
                chat_id = config.ChatId,
                text = message,
                parse_mode = "HTML"
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

        private static ArkhamResponse? ConvertContent(string content)
        {
            return JsonConvert.DeserializeObject<ArkhamResponse>(content);

        }

        private static (string, string) SelectEmoji(string alertname)
        {
            if (string.IsNullOrEmpty(alertname)) return ("", "");
            return alertname.Contains("BUY", StringComparison.OrdinalIgnoreCase) ? ("🟢", "BUY") : alertname.Contains("InFlow", StringComparison.OrdinalIgnoreCase) ? ("🟢", "BUY") : ("🔴", "SELL");
        }

        private static string Formatmessage(ArkhamResponse arkhamResponse)
        {
            var transfer = arkhamResponse.Transfer;
            var (directionEmoji, actionText) = SelectEmoji(arkhamResponse.AlertName);

            // string actionText = transfer?.Type.ToLower() == "buy" ? "BUY" : "SELL";

            // Helper to format addresses with Arkham-style links
            string FormatAddress(AddressInfo? addr)
            {
                if (addr == null) return "<code>Unknown</code>";
                var labelName = addr.ArkhamLabel?.Name;
                var address = addr.Address ?? "Unknown";

                return !string.IsNullOrWhiteSpace(labelName)
                    ? $"<a href=\"https://intel.arkm.com/explorer/address/{address}\">{labelName}</a>"
                    : $"<code>{address}</code>";
            }

            // Format addresses with Arkham-style links
            string fromAddresses = FormatAddress(transfer?.FromAddress);
            string toAddresses = FormatAddress(transfer?.ToAddress);

            // Format value
            string valueText = $"{transfer?.UnitValue:N6} {transfer?.TokenSymbol} (${transfer?.HistoricalUSD:N2})";

            // Format links
            string txLink = $"<a href=\"https://intel.arkm.com/explorer/tx/{transfer?.TransactionHash}\">View on Arkham</a>";
            string blockLink = transfer?.Chain.ToLower() == "bitcoin"
                ? $"<a href=\"https://blockstream.info/tx/{transfer.TransactionHash}\">View on Blockstream</a>"
                : $"<a href=\"https://etherscan.io/tx/{transfer?.TransactionHash}\">View on Etherscan</a>";

            // string pauseLink = $"<a href=\"https://intel.arkm.com/alerts/{alert.Id}?action=pause\">Pause Alert</a>";

            // Final message
            string messageText = $@"
            {directionEmoji} <b>{actionText}</b>
            <b>From:</b> {fromAddresses} and others
            <b>To:</b> {toAddresses}
            <b>Value:</b> {valueText}
            <b>Network:</b> {transfer?.Chain}
            <b>Time:</b> {transfer?.BlockTimestamp:yyyy-MM-dd HHmm UTC}
            {txLink} | {blockLink} ";

            return messageText;
        }


    }
}