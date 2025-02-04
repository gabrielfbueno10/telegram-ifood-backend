using System.Net.Http;
using TelegramIfood.API.Extensions;
using TelegramIfood.API.Models.Ifood;

namespace TelegramIfood.API.Services.Ifood;

public class IfoodAuthService : BaseHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly AppSettings _appSettings;
    public IfoodAuthService(HttpClient httpClient, AppSettings appSettings)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new(appSettings.ApisUrls.MerchantUrl);
        _appSettings = appSettings;
    }
    public async Task<IfoodAuthResponse> CreateSessionAsync()
    {
        var response = await _httpClient.PostAsync("/authentication/v1.0/oauth/token", ObterConteudoForm(new
        {
            grantType = "client_credentials",
            clientId = _appSettings.IfoodSettings.ClientId,
            clientSecret = _appSettings.IfoodSettings.ClientSecret
        }));

        var result = await DeserializeResponse<IfoodAuthResponse>(response);

        IfoodTokenValidatorExtension.Auth = result;
        IfoodTokenValidatorExtension.CreatedAt = DateTime.UtcNow;

        return result;
    }
}
