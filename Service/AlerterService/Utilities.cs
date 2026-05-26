
using AlerterService;
using AlerterService.Models;
using Newtonsoft.Json;

namespace Alerter.Utility;

public class Utiliy : IUtility
{

    // // Helper to format addresses with Arkham-style links
    public string FormatAddress(List<AddressInfo?> addr)
    {
        if (addr?.Count == 0 || addr == null) return "<code>Unknown</code>";
        var addcount = addr?.Count;

        var address = addr?.FirstOrDefault()?.Address ?? "Unknown";
        string addressresponse = addcount > 1 ? $"{address} and {addcount - 1} others" : address;

        return addressresponse;
    }

    public ArkhamResponse? ConvertContent(string content)
    {
        return JsonConvert.DeserializeObject<ArkhamResponse>(content);

    }


    public string Formatmessage(ArkhamResponse arkhamResponse)
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


    private static (string, string) SelectEmoji(string alertname)
    {
        if (string.IsNullOrEmpty(alertname)) return ("", "");
        return alertname.Contains("BUY", StringComparison.OrdinalIgnoreCase) ? ("🟢", "BUY") : alertname.Contains("InFlow", StringComparison.OrdinalIgnoreCase) ? ("🟢", "BUY") : ("🔴", "SELL");
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