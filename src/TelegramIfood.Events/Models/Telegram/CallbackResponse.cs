using System.Text.Json;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramIfood.Events.Models.Telegram;
public class CallbackResponse
{
    public CallbackResponse(string mensagem, params LinhaBotoes[]? linhas)
    {
        Mensagem = mensagem;

        Botoes = linhas?.Length > 0 ? new(linhas.Select(x => x.Botoes)) : null;
    }
    public string Mensagem { get; set; }
    public InlineKeyboardMarkup? Botoes { get; set; }
}


public class LinhaBotoes
{
    public LinhaBotoes(params Botao[] botoes)
    {
        if (botoes is null) throw new Exception("Não é possivel criar uma linha sem botoes");

        Botoes = botoes.Select(x =>
        {
            var btn = InlineKeyboardButton.WithCallbackData(text: x.Texto, callbackData: x.Valor);

            if (x.Url is not null) btn.Url = x.Url;

            return btn;
        }).ToArray();
    }

    public InlineKeyboardButton[] Botoes { get; set; }
}

//public record Botao(string Texto = "Escolha uma opção abaixo", string? Valor = null, string? url = null);

public class Botao
{
    public Botao(string mensagem, object? valor = null, string? url = null)
    {
        if (valor is null && url is null) throw new Exception("Configura direito esse botao");

        Texto = mensagem;

        if (valor != null)
        {
            var typeName = valor.GetType().Name;
            var json = JsonSerializer.Serialize(valor);

            Valor = $"{typeName}|{json}";
        }

        Url = url;

    }
    public string Texto { get; set; } = "";
    public string? Valor { get; set; }
    public string? Url { get; set; }
}