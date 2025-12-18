namespace FCG.Games.Infra.Persistence.Config
{
    public class SqlServerOptions
    {
        public string ConnectionStrings { get; set; } = string.Empty;
        public int MaxRetryCount { get; set; } = 5;
        public int MaxRetryDelaySeconds { get; set; } = 10;
        public bool EnableRetryOnFailure { get; set; } = true;
    }
}
