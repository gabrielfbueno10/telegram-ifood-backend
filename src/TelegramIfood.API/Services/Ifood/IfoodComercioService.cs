using TelegramIfood.API.Extensions;
using TelegramIfood.Events.Models.Ifood;

namespace TelegramIfood.API.Services.Ifood;

public class IfoodComercioService : BaseHttpClient
{
    private readonly HttpClient _httpClient;
    public IfoodComercioService(HttpClient httpClient, AppSettings appSettings)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new(appSettings.ApisUrls.MerchantUrl);
    }

    public async Task<IfoodComercioStatus> StatusComercioAsync(Guid merchantId)
    {
        var response = await _httpClient.GetAsync($"/merchant/v1.0/merchants/{merchantId}/status");

        return (await DeserializeResponse<IEnumerable<IfoodComercioStatus>>(response))?.FirstOrDefault()
            ?? new()
            {
                message = new()
                {
                    title = "Loja fechada",
                    subtitle = "Sua loja ainda precisa ser configurada"
                }
            };
    }


    public async Task<IEnumerable<IfoodInterrupcaoComercio>> ListarInterrupcoesAsync(Guid merchantId)
    {
        var response = await _httpClient.GetAsync($"/merchant/v1.0/merchants/{merchantId}/interruptions");

        return await DeserializeResponse<IEnumerable<IfoodInterrupcaoComercio>>(response) ?? new List<IfoodInterrupcaoComercio>();
    }

    public async Task CriarInterrupcaoAsync(Guid merchantId, DateTime start, DateTime end, string motivo)
    {
        var response = await _httpClient.PostAsJsonAsync($"/merchant/v1.0/merchants/{merchantId}/interruptions", new
        {
            start,
            end,
            description = motivo
        });
    }

    public async Task DeletarInterruptionAsync(Guid merchantId, Guid interruptionId)
    {
        var response = await _httpClient.DeleteAsync($"/merchant/v1.0/merchants/{merchantId}/interruptions/{interruptionId}");
    }
}