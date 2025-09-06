using Domain.Enums;

namespace Domain.Entities;

public class Promotion
{

    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int DiscountPercentage { get; set; }
    public GameGenre TargetGenre { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public Promotion(string name, string description, int discountPercentage, GameGenre targetGenre, DateTime startDate, DateTime endDate)
    {
        ValidateName(name);
        ValidateDescription(description);
        ValidateDiscountPercentage(discountPercentage);
        ValidateStartDate(startDate);
        ValidateEndDate(endDate, startDate);
        Name = name;
        Description = description;
        DiscountPercentage = discountPercentage;
        TargetGenre = targetGenre;
        StartDate = startDate;
        EndDate = endDate;
    }
     public bool IsActive()
    {
        return DateTime.UtcNow >= StartDate && DateTime.UtcNow <= EndDate;
    }
    public decimal CalculateDiscountedPrice(Game game)
    {
        if (!IsActive()) return game.Price;
        if (!game.Genre.Contains(TargetGenre)) return game.Price;
        return game.Price * (1 - DiscountPercentage / 100m);
    }

    private Promotion() { } // For EF Core

    public static void ValidateName(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new Exception("Nome não pode ser vazio ou nulo.");
    }
    public static void ValidateDescription(string? description)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new Exception("Descrição não pode ser vazio ou nulo.");
    }
    public static void ValidateDiscountPercentage(int? discountPercentage)
    {
        if (discountPercentage is null or < 0 or > 100)
            throw new ArgumentOutOfRangeException("O desconto precisa estar entre 0 e 100.");
    }
    public static void ValidateStartDate(DateTime? startDate)
    {
        if (startDate is null)
            throw new Exception("Data de início inválida.");
    }
    public static void ValidateEndDate(DateTime? endDate, DateTime startDate)
    {
        if (endDate is null || endDate <= startDate)
            throw new Exception("Data de término inválida.");
    }
}
