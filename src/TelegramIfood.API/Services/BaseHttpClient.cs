using System.Text.Json;
using System.Text;
using TelegramIfood.API.Extensions;

namespace TelegramIfood.API.Services;
public class BaseHttpClient
{
    protected StringContent ObterConteudoJson(object dado)
    {
        return new StringContent(
            JsonSerializer.Serialize(dado),
            Encoding.UTF8,
            "application/json");
    }
    protected async Task<T?> DeserializeResponse<T>(HttpResponseMessage response)
    {
        if ((int)response.StatusCode == 401)
        {
            IfoodTokenValidatorExtension.Auth.accessToken = string.Empty;
            return default;
        }
        if ((int)response.StatusCode == 204) return default;

        var options = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        };

        var json = await response.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<T>(json, options);
    }

    protected FormUrlEncodedContent ObterConteudoForm(object dado)
    {
        var result = new Dictionary<string, string>();

        foreach (var p in dado.GetType().GetProperties())
        {
            var val = p.GetValue(dado, null);

            result.Add(p.Name, val?.ToString());
        }

        return new(result.ToList());
    }
}
