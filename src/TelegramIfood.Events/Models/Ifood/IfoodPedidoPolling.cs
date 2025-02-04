namespace TelegramIfood.Events.Models.Ifood;

public class IfoodPedidoPolling
{
    public string id { get; set; }
    public string code { get; set; }
    public string fullCode { get; set; }
    public Guid orderId { get; set; }
    public Guid merchantId { get; set; }
    public DateTime createdAt { get; set; }
    public bool EhPedidoRecebido() => code.Equals("PLC", StringComparison.OrdinalIgnoreCase);
    public bool EhCancelamentoRecebido() => code.Equals("CAN", StringComparison.OrdinalIgnoreCase);
}