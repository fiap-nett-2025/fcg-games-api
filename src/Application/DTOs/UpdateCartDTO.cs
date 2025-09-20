namespace Application.DTOs;

public class UpdateCartDTO
{
    public string? UserId { get; set; }
    public List<int>? GameIds { get; set; } = new();
}
