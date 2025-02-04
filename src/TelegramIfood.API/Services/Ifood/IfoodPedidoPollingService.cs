using TelegramIfood.API.Extensions;
using TelegramIfood.API.Models.Ifood;
using TelegramIfood.Events.Models.Ifood;

namespace TelegramIfood.API.Services.Ifood;

public class IfoodPedidoPollingService : BaseHttpClient
{
    private readonly HttpClient _httpClient;

    public IfoodPedidoPollingService(HttpClient httpClient, AppSettings appSettings)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new(appSettings.ApisUrls.MerchantUrl);
    }

    public async Task<IEnumerable<IfoodPedidoPolling>> GetPedidoPollingsAsync()
    {
        var response = await _httpClient.GetAsync("/order/v1.0/events:polling");

        return await DeserializeResponse<IEnumerable<IfoodPedidoPolling>>(response) ?? new List<IfoodPedidoPolling>();
    }

    public async Task EnviarRecebimentoPedidoAsync(IfoodPedidoPolling pedidoPolling)
    {
        await _httpClient.PostAsJsonAsync("/order/v1.0/events/acknowledgment", new object[] { new { pedidoPolling.id } });
    }

    public async Task EnviarRecebimentoPedidoAsync(IEnumerable<IfoodPedidoPolling> pedidoPollings)
    {
        await _httpClient.PostAsJsonAsync("/order/v1.0/events/acknowledgment", pedidoPollings.Select(x => new { x.id }));
    }
}

