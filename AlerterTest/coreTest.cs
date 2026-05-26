
using Moq;
using Service.Models;
using AlerterService;
using System.Threading.Tasks;
using AlerterService.Models;
using Moq.Protected;
namespace AlerterTest;

public class AlerterCoreTest
{

    private readonly Mock<IHttpClientFactory> _client;
    private readonly Mock<IUtility> _helpers;

    private readonly IAlerterService _service;
    private readonly TelegramConfig _config;

    public AlerterCoreTest()
    {
        _client = new Mock<IHttpClientFactory>();
        _helpers = new Mock<IUtility>();
        _service = new AlertService(_client.Object, _helpers.Object);
        _config = new TelegramConfig
        {
            Token = Guid.NewGuid().ToString(),
            ChatId = "1234567"
        };
    }

    [Fact]
    public async Task SendToTelegram_Should_ReturnFalse_IfContentEmtpy()
    {
        string Content = string.Empty;

        var result = await _service.SendToTelegram(_config, Content);
        Assert.False(result);

    }

    [Fact]
    public async Task SendToTelegram_Should_ReturnFalse_IfObjectIsNull()
    {
        string Content = "string Empty";
        _helpers.Setup(x => x.ConvertContent(It.IsAny<string>())).Returns((ArkhamResponse)null!);
        var result = await _service.SendToTelegram(_config!, Content);
        Assert.False(result);

    }

     [Fact]
    public async Task SendToTelegram_Should_ReturnFalse_IfConfigIsNull()
    {
        string Content = "string Empty";

        _helpers.Setup(x => x.ConvertContent(It.IsAny<string>())).Returns(new ArkhamResponse());

        _helpers.Setup(x => x.Formatmessage(It.IsAny<ArkhamResponse>())).Returns("Test");
        
       var result =  await Assert.ThrowsAsync<ArgumentNullException>(() => _service.SendToTelegram(null!, Content));

        Assert.NotNull(result);
        Assert.Equal("Value cannot be null. (Parameter 'config')", result.Message);
        

    }

    [Fact]
    public async Task SendToTelegram_Should_ReturnTrue_IfSentSuccessfully()
    {
        string Content = "string Empty";

        _helpers.Setup(x => x.ConvertContent(It.IsAny<string>())).Returns(new ArkhamResponse());

        _helpers.Setup(x => x.Formatmessage(It.IsAny<ArkhamResponse>())).Returns("Test");

        var httphandler = new Mock<HttpMessageHandler>();

        httphandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()).ReturnsAsync(new HttpResponseMessage
        {
            StatusCode = System.Net.HttpStatusCode.OK,
            Content = new StringContent("Sent")
        });

        //http client with custom handler
        var client = new HttpClient(httphandler.Object);


        _client.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(client);


        Assert.True(await _service.SendToTelegram(_config!, Content));

    }



}