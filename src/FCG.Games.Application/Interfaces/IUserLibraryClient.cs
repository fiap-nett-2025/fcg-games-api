namespace FCG.Games.Application.Interfaces;

public interface IUserLibraryClient
{
    Task<List<string>> GetOwnedGameIdsAsync(Guid userId, string jwt);
}
