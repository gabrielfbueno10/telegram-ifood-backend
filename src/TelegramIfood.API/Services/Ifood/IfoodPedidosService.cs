using TelegramIfood.API.Extensions;
using TelegramIfood.API.Models.Ifood;
using TelegramIfood.Events.Models.Ifood;

namespace TelegramIfood.API.Services.Ifood;

public class IfoodPedidosService : BaseHttpClient
{
    private readonly HttpClient _httpClient;
    public IfoodPedidosService(HttpClient httpClient, AppSettings appSettings)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new(appSettings.ApisUrls.MerchantUrl);
    }

    public async Task<IfoodPedidoDetalhe?> GetPedidoDetalhesAsync(Guid orderId)
    {
        var response = await _httpClient.GetAsync($"/order/v1.0/orders/{orderId}");

        return await DeserializeResponse<IfoodPedidoDetalhe>(response);
    }

    public async Task ConfirmarPedidoAsync(Guid orderId)
    {
        await _httpClient.PostAsync($"/order/v1.0/orders/{orderId}/confirm", null);
    }

    public async Task ComecarPreparacaoAsync(Guid orderId)
    {
        await _httpClient.PostAsync($"/order/v1.0/orders/{orderId}/startPreparation", null);
    }

    public async Task PedidoProntoParaColetaAsync(Guid orderId)
    {
        await _httpClient.PostAsync($"/order/v1.0/orders/{orderId}/readyToPickup", null);
    }

    public async Task DespacharPedidoAsync(Guid orderId)
    {
        await _httpClient.PostAsync($"/order/v1.0/orders/{orderId}/startPreparation", null);
    }

    public async Task CancelarPedidoAsync(Guid orderId)
    {
        await _httpClient.PostAsJsonAsync($"/order/v1.0/orders/{orderId}/requestCancellation", new
        {
            cancellationCode = 509,
            reason = "Cancelado pelo sistema"
        });
    }

    public async Task AceitarCancelamentoPedidoAsync(Guid orderId)
    {
        await _httpClient.PostAsync($"/order/v1.0/orders/{orderId}/acceptCancellation", null);
    }

    public async Task RecusarCancelamentoPedidoAsync(Guid orderId)
    {
        await _httpClient.PostAsync($"/order/v1.0/orders/{orderId}/denyCancellation", null);
    }
}
