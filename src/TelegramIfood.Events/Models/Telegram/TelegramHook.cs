namespace TelegramIfood.Events.Models.Telegram;

public class TelegramHook
{
    public int update_id { get; set; }
    public HookCallbackQuery callback_query { get; set; }
    public HookMessage message { get; set; }
}

public class HookCallbackQuery
{
    public string id { get; set; }
    public HookFrom from { get; set; }
    public HookMessage message { get; set; }
    public string chat_instance { get; set; }
    public string data { get; set; }
}

public class HookFrom
{
    public long id { get; set; }
    public bool is_bot { get; set; }
    public string first_name { get; set; }
    public string username { get; set; }
    public string language_code { get; set; }
}

public class HookMessage
{
    public int message_id { get; set; }
    public HookFrom from { get; set; }
    public int date { get; set; }
    public string text { get; set; }
}