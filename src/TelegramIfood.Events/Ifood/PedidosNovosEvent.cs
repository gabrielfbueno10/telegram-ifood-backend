﻿using MediatR;
using TelegramIfood.Events.Models.Ifood;

namespace TelegramIfood.Events.Ifood;

public class PedidosNovosEvent : INotification
{
    public IEnumerable<IfoodPedidoPolling> Pedidos { get; set; }
}
