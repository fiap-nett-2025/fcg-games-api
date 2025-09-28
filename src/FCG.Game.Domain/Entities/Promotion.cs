using FCG.Game.Domain.Enums;

namespace FCG.Game.Domain.Entities;

public class Promotion
{
    public int Id { get; private set; }
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public int DiscountPercentage { get; private set; }
    public GameGenre TargetGenre { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }

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
    private Promotion() { } // For EF Core
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

    #region Updaters
    public void UpdateName(string name)
    {
        ValidateName(name);
        Name = name;
    }

    public void UpdateDescription(string description)
    {
        ValidateDescription(description);
        Description = description;
    }

    public void UpdateDiscountPercentage(int discountPercentage)
    {
        ValidateDiscountPercentage(discountPercentage);
        DiscountPercentage = discountPercentage;
    }

    public void UpdateTargetGenre(GameGenre targetGenre)
    {
        TargetGenre = targetGenre;    
    }
    public void UpdateStartDate(DateTime startDate)
    {
        ValidateStartDate(startDate);
        StartDate = startDate;
    }

    public void UpdateEndDate(DateTime endDate)
    {
        ValidateEndDate(endDate, StartDate);
        EndDate = endDate;
    }
    #endregion

    #region Validators
    public static void ValidateName(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Nome não pode ser vazio ou nulo.");
    }
    public static void ValidateDescription(string? description)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Descrição não pode ser vazio ou nulo.");
    }
    public static void ValidateDiscountPercentage(int? discountPercentage)
    {
        if (discountPercentage is null or < 0 or > 100)
            throw new ArgumentOutOfRangeException("O desconto precisa estar entre 0 e 100.");
    }
    public static void ValidateStartDate(DateTime? startDate)
    {
        if (startDate is null)
            throw new ArgumentException("Data de início inválida.");
    }
    public static void ValidateEndDate(DateTime? endDate, DateTime startDate)
    {
        if (endDate is null || endDate <= startDate)
            throw new ArgumentException("Data de término inválida.");
    }
    #endregion
}
