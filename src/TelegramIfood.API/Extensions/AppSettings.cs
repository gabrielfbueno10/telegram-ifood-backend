namespace TelegramIfood.API.Extensions;

public class AppSettings
{
    public IFoodSettings IfoodSettings { get; set; }
    public ApisUrls ApisUrls { get; set; }
    public BotConfiguration TelegramSettings { get; set; }

    public static AppSettings BuildFromConfiguration(IConfiguration configuration)
    {
        return configuration.Get<AppSettings>();
    }
}

public class IFoodSettings
{
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
}

public class ApisUrls
{
    public string MerchantUrl { get; set; }
}


public class BotConfiguration
{
    public static readonly string Configuration = "BotConfiguration";

    public string BotToken { get; init; } = default!;
    public string HostAddress { get; init; } = default!;
    public string Route { get; init; } = default!;
    public string SecretToken { get; init; } = default!;
}
