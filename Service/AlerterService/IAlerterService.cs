using AlerterService.Models;
using Service.Models;
namespace AlerterService;

public interface IAlerterService
{
    public Task<bool> SendToTelegram(TelegramConfig config, string Content);

}