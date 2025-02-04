namespace TelegramIfood.Events.Models.Ifood;

public static class PedidoStatusIfoodConstant
{
    public static readonly string NOVO_PEDIDO_IFOOD = "PLC"; //Novo pedido na plataforma
    public static readonly string PEDIDO_CONFIRMADO = "CFM"; //Pedido foi confirmado e será preparado
    public static readonly string PEDIDO_PRONTO_RETIRAR = "RTP"; //Indica que o pedido está pronto para ser retirado (Pra Retirar)
    public static readonly string PEDIDO_DESPACHADO = "DSP"; //Indica que o pedido saiu para entrega (Delivery)
    public static readonly string PEDIDO_CONCLUIDO = "CON"; //Pedido foi concluído
    public static readonly string PEDIDO_CANCELADO = "CAN"; //Pedido foi cancelado
    public static readonly string PEDIDO_PREPARANDO = "PRS"; //Pedido foi cancelado
}