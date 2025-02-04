using TelegramIfood.API.Models.Ifood;

namespace TelegramIfood.API.Extensions;

public static class IfoodTokenValidatorExtension
{
    public static DateTime CreatedAt { get; set; }
    public static IfoodAuthResponse Auth { get; set; }
    public static bool EstaValido() => !string.IsNullOrEmpty(Auth?.accessToken) && CreatedAt.AddMinutes(Auth.expiresIn) >= DateTime.UtcNow;
    public static void InvalidarToken()
    {
        Auth = new();
    }
}