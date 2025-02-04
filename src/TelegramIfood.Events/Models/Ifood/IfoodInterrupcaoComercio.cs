using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramIfood.Events.Models.Ifood;

public class IfoodInterrupcaoComercio
{
    public Guid id { get; set; }
    public DateTime start { get; set; }
    public DateTime end { get; set; }
    public string description { get; set; }
}
