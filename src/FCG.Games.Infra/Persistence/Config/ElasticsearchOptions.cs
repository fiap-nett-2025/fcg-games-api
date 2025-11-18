namespace FCG.Games.Infra.Persistence.Config;

public class ElasticsearchOptions
{
    public string Url { get; set; } = string.Empty;
    public string IndexName {  get; set; } = string.Empty;
}
