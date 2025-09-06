using Application.DTOs;
using Application.Exceptions;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services;

public class GameService : IGameService
{
    private readonly IGameRepository _gameRepository;

    public GameService(IGameRepository gameRepository)
    {
        _gameRepository = gameRepository;
    }

    public async Task<IEnumerable<GameDTO>> GetAllGamesAsync()
    {
        var games = await _gameRepository.GetAllAsync();
        return games.Select(game => MapToDto(game)).ToList();
    }

    public async Task<GameDTO> GetGameByIdAsync(int id)
    {
        var game = await _gameRepository.GetBy(game => game.Id.Equals(id));
        if (game == null)
            throw new NotFoundException("Jogo não encontrado.");

        return MapToDto(game);
    }

    public async Task<GameDTO> CreateGameAsync(CreateGameDTO model)
    {
        var existingGame = await _gameRepository.GetBy(g => g.Title.ToUpper() == model.Title.ToUpper());
        if (existingGame is not null)
            throw new BusinessErrorDetailsException($"Jogo com o título '{model.Title}' já existe.");

        try
        {
            var game = new Game(model.Title, model.Price, model.Description, model.Genre);
            await _gameRepository.AddAsync(game);
            
            return MapToDto(game);
        }
        catch (ArgumentException ex)
        {
            throw new BusinessErrorDetailsException(ex.Message);
        }
    }

    public async Task UpdateGameAsync(int id, UpdateGameDTO model)
    {
        var game = await _gameRepository.GetBy(g => g.Id.Equals(id));
        if (game is null)
            throw new NotFoundException($"Jogo {id} não encontrado.");

        var ExistingGame = await _gameRepository.GetBy(g => g.Title.ToUpper() == model.Title.ToUpper() && g.Id != id);

        if (ExistingGame is not null)
            throw new BusinessErrorDetailsException($"Jogo com o título '{model.Title}' já existe.");  

        if (!string.IsNullOrWhiteSpace(model.Title))
            game.UpdateTitle(model.Title);

        if (model.Price.HasValue)
            game.UpdatePrice(model.Price.Value);

        if (!string.IsNullOrWhiteSpace(model.Description))
            game.UpdateDescription(model.Description);

        if (model.Genre is not null && model.Genre.Count != 0)
            game.UpdateGenre(model.Genre);

        await _gameRepository.UpdateAsync(game);
    }

    public async Task DeleteGameAsync(int id)
    {
        var game = await _gameRepository.GetBy(g => g.Id.Equals(id));
        if (game is null)
            throw new NotFoundException($"Jogo {id} não encontrado.");

        await _gameRepository.DeleteAsync(id);
    }

    private static GameDTO MapToDto(Game game) => new GameDTO
    {
        Id = (int)game.Id,
        Title = game.Title,
        Price = game.Price,
        Description = game.Description,
        Genre = game.Genre
    };
}