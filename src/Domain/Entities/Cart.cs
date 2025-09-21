namespace Domain.Entities;

public class Cart
{
    public string UserId { get; private set; } = null!;
    public List<int> GameIds { get; private set; } = new List<int>();
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public Cart(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId)) 
          throw new ArgumentException("Usuário não pode ser nulo ou estar em branco.");

        UserId = userId;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    private Cart() { } // For EF Core

    public void AddGame(int gameId)
    {
        if (GameIds.Contains(gameId))
            throw new ArgumentException("Jogo já está no carrinho.");
        
        GameIds.Add(gameId);
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveGame(int gameId)
    {
        if (!GameIds.Contains(gameId))
            throw new ArgumentException("Jogo não está no carrinho.");
        
        GameIds.Remove(gameId);
        UpdatedAt = DateTime.UtcNow;
    }

    public void ClearCart()
    {
        GameIds.Clear();
        UpdatedAt = DateTime.UtcNow;
    }
}
