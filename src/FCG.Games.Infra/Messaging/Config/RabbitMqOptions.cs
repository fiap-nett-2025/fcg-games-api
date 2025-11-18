namespace FCG.Games.Infra.Messaging.Config;

public class RabbitMqOptions
{
    public string HostName { get; set; } = string.Empty;
    public int Port { get; set; }
    public string VirtualHost { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    public bool UseSsl { get; set; } = false;
    public string ClientProvidedName { get; set; } = string.Empty;
}
