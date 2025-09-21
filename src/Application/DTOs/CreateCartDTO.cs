namespace Application.DTOs;

public class CreateCartDTO
{
    public required string UserId { get; set; }
    public required List<int> GameIds { get; set; }
}
