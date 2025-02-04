namespace TelegramIfood.API.Models.Ifood;

public class IfoodAuthResponse
{
    public string accessToken { get; set; }
    public string type { get; set; }
    public int expiresIn { get; set; }
}
