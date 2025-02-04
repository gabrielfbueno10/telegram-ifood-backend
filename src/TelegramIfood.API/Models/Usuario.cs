namespace TelegramIfood.API.Models;

public class Usuario
{
    public Guid Id { get; set; }
    public string Nome { get; set; }
    public long IdTelegram { get; set; }
    public bool Ativo { get; set; }
    public Guid EstabelecimentoId { get; set; }
    public DateTime CreatedAt { get; set; }
}