using AlerterService.Models;
using Service.Models;
namespace AlerterService;

public interface IAlerterService
{
    public Task<bool> SendToTelegram(TelegramConfig config, string Content);

}

public interface IUtility
{
    public string FormatAddress(List<AddressInfo?> addr);

    public ArkhamResponse? ConvertContent(string content);

    public string Formatmessage(ArkhamResponse arkhamResponse);
}