using TelegramIfood.Events.Models.Telegram;

namespace TelegramIfood.API.Utils;

public static class TelegramResponseHelper
{
    public static CallBackResponseBuilder GerarResponse(string mensagem) { return new CallBackResponseBuilder(mensagem); }
    public static CallBackResponseBuilder AddLinha(this CallBackResponseBuilder builder)
    {
        builder.Botoes.Add(new());

        return builder;
    }
    public static CallBackResponseBuilder AddBotao(this CallBackResponseBuilder builder, Botao botao)
    {
        if (builder.Botoes.Count <= 0) builder.Botoes.Add(new());
        builder.Botoes.Last().Add(botao);
        return builder;
    }

    public static CallBackResponseBuilder AddBotao(this CallBackResponseBuilder builder, string texto, object valor, string? url = null)
    {
        if(builder.Botoes.Count <= 0) builder.Botoes.Add(new());
        builder.Botoes.Last().Add(new(texto, valor));
        return builder;
    }

    public static CallBackResponseBuilder AtualizarMensagem(this CallBackResponseBuilder builder, string mensagem)
    {
        builder.Mensagem = mensagem;
        return builder;
    }
}

public class CallBackResponseBuilder
{
    public CallBackResponseBuilder(string mensagem)
    {
        Botoes = new();
        Mensagem = mensagem;
    }
    public string Mensagem { get; set; }
    public List<List<Botao>> Botoes { get; set; }
    public CallbackResponse ToResponse()
    {
        return new(Mensagem,
            Botoes?.Count > 0 ?
            Botoes
                .Where(x => x?.Count > 0)
                .Select(x => new LinhaBotoes(x.ToArray()))
                .ToArray() : null);
    }
}