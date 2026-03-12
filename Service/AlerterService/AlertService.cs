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

            //Normalize address
            var getfromaddress = GetFromAddress(arkhamResponse.Transfer);
            var gettoAddresses = GetToAddress(arkhamResponse.Transfer);

            //format address 
            string fromAddresses = FormatAddress(getfromaddress);
            string toAddresses = FormatAddress(gettoAddresses);


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
            {directionEmoji} <b>{arkhamResponse.AlertName}</b>
            <b>From:</b> {fromAddresses} 
            <b>To:</b> {toAddresses}
            <b>Value:</b> {valueText}
            <b>Network:</b> {transfer?.Chain}
            <b>Time:</b> {transfer?.BlockTimestamp:yyyy-MM-dd HHmm UTC}
            {txLink} | {blockLink} ";

            return messageText;
        }
        // Helper to format addresses with Arkham-style links
        private static string FormatAddress(List<AddressInfo?> addr)
        {
            if (addr?.Count == 0 || addr == null) return "<code>Unknown</code>";
            var addcount = addr?.Count;

            var address = addr?.FirstOrDefault()?.Address ?? "Unknown";
            string addressresponse = addcount > 1 ? $"{address} and {addcount - 1} others" : address;

            return addressresponse;
        }

        private static List<AddressInfo?> GetFromAddress(Transfer? transfer)
        {
            ArgumentNullException.ThrowIfNull(transfer);
            if (transfer.FromAddresses != null && transfer.FromAddresses.Any())
            {
                return transfer.FromAddresses.Where(x => x.Address != null).Select(x => x.Address).ToList();
            }
            if (transfer.FromAddress != null) return [transfer.FromAddress];
            return [];
        }

        private static List<AddressInfo?> GetToAddress(Transfer? transfer)
        {
            ArgumentNullException.ThrowIfNull(transfer);
            if (transfer.ToAddresses != null && transfer.ToAddresses.Any())
            {
                return transfer.ToAddresses.Where(x => x.Address != null).Select(x => x.Address).ToList();
            }
            if (transfer.ToAddress != null) return [transfer.ToAddress];

            return [];
        }
    }
}