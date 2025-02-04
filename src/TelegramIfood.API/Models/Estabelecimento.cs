namespace TelegramIfood.API.Models;

public class Estabelecimento
{
    public Guid Id { get; set; }
    public string Nome { get; set; }
    public Guid IfoodMerchantId { get; set; }
    public bool Ativo { get; set; }
}
