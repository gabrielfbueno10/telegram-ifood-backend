using System.Net.Http.Headers;
using TelegramIfood.API.Extensions;

namespace TelegramIfood.API.Services.Ifood.Handlers;

public class IfoodAuthorizationDelegatingHandler : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (!IfoodTokenValidatorExtension.EstaValido()) IfoodTokenValidatorExtension.InvalidarToken();

        if (!string.IsNullOrEmpty(IfoodTokenValidatorExtension.Auth.accessToken))
        {
            request.Headers.Authorization = new("bearer", IfoodTokenValidatorExtension.Auth.accessToken);
            //request.Headers.Add("Authorization", new List<string>() { IfoodTokenValidatorExtension.Auth.accessToken });
        }

        return base.SendAsync(request, cancellationToken);
    }
}