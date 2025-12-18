namespace FCG.Games.Application.DTOs;

public class MessageDTO
{
    public string UserId { get; set; } = string.Empty;
    public List<string> GamesId { get; set; } = [];
}
