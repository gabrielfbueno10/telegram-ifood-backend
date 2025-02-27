﻿namespace TelegramIfood.Events.Models.Ifood;

public class IfoodComercioStatus
{
    public string operation { get; set; }
    public string salesChannel { get; set; }
    public bool available { get; set; }
    public string state { get; set; }
    public Reopenable reopenable { get; set; }
    public Validation[] validations { get; set; }
    public Message message { get; set; }
}

public class Reopenable
{
    public object identifier { get; set; }
    public object type { get; set; }
    public bool reopenable { get; set; }
}

public class Message
{
    public string title { get; set; }
    public string subtitle { get; set; }
    public object description { get; set; }
    public object priority { get; set; }
}

public class Validation
{
    public string id { get; set; }
    public string code { get; set; }
    public string state { get; set; }
    public Message1 message { get; set; }
}

public class Message1
{
    public string title { get; set; }
    public string subtitle { get; set; }
    public string description { get; set; }
    public int priority { get; set; }
}

