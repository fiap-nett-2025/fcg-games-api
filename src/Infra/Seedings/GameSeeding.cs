namespace Infra.Seedings;

using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
public static class GameSeeding
{
    public static async Task SeedAsync(DbContext context)
    {
        try
        {
            Console.WriteLine("Starting game seed...");

            var gamesToSeed = new List<Game>()
                {
                    new Game(
                        "The Legend of Zelda: Breath of the Wild",
                        59.99m,
                        "Um jogo de aventura em mundo aberto onde você explora o reino de Hyrule.",
                        new List<GameGenre> { GameGenre.Adventure, GameGenre.Action, GameGenre.RPG }
                    ),
                    new Game(
                        "Super Mario Odyssey",
                        49.99m,
                        "Uma aventura 3D com Mario em diversos mundos para resgatar a Princesa Peach.",
                        new List<GameGenre> { GameGenre.Platformer, GameGenre.Adventure }
                    ),
                    new Game(
                        "Minecraft",
                        26.95m,
                        "Um jogo de construção e sobrevivência em um mundo gerado aleatoriamente.",
                        new List<GameGenre> { GameGenre.Sandbox, GameGenre.Survival, GameGenre.Adventure }
                    ),
                    new Game(
                        "The Witcher 3: Wild Hunt",
                        39.99m,
                        "Um RPG de ação em mundo aberto onde você joga como Geralt de Rivia em busca de sua filha adotiva.",
                        new List<GameGenre> { GameGenre.RPG, GameGenre.Action, GameGenre.Adventure }
                    ),
                    new Game(
                        "Dark Souls III",
                        29.99m,
                        "Um RPG de ação desafiador com combate intenso e exploração em um mundo sombrio.",
                        new List<GameGenre> { GameGenre.RPG, GameGenre.Action, GameGenre.Horror }
                    ),
                    new Game(
                        "God of War 3",
                        39.99m,
                        "Uma reinvenção da série God of War, focada na relação entre Kratos e seu filho Atreus.",
                        new List<GameGenre> { GameGenre.Action, GameGenre.Adventure, GameGenre.RPG }
                    ),
                    new Game(
                        "Hollow Knight",
                        14.99m,
                        "Um jogo de plataforma e ação em um mundo subterrâneo cheio de segredos e desafios.",
                        new List<GameGenre> { GameGenre.Platformer, GameGenre.Action, GameGenre.Adventure }
                    ),
                    new Game(
                        "Red Dead Redemption 2",
                        59.99m,
                        "Um jogo de ação e aventura no velho oeste americano.",
                        new List<GameGenre> { GameGenre.Action, GameGenre.Adventure, GameGenre.Sandbox }
                    ),
                    new Game(
                        "Cyberpunk 2077",
                        49.99m,
                        "Um RPG de ação em um futuro distópico cyberpunk.",
                        new List<GameGenre> { GameGenre.RPG, GameGenre.Action, GameGenre.Adventure }
                    ),
                    new Game(
                        "Elden Ring",
                        59.99m,
                        "Um RPG de ação em mundo aberto desenvolvido pela FromSoftware.",
                        new List<GameGenre> { GameGenre.RPG, GameGenre.Action, GameGenre.Adventure }
                    ),
                    new Game(
                        "FIFA 23",
                        59.99m,
                        "Simulador de futebol com times e jogadores reais.",
                        new List<GameGenre> { GameGenre.Sports, GameGenre.Simulation }
                    ),
                    new Game(
                        "Call of Duty: Modern Warfare II",
                        69.99m,
                        "Jogo de tiro em primeira pessoa com campanha e multiplayer.",
                        new List<GameGenre> { GameGenre.FPS, GameGenre.Action }
                    ),
                    new Game(
                        "Resident Evil 4 Remake",
                        59.99m,
                        "Remake do clássico jogo de survival horror.",
                        new List<GameGenre> { GameGenre.Horror, GameGenre.Action, GameGenre.Adventure }
                    ),
                    new Game(
                        "Forza Horizon 5",
                        59.99m,
                        "Jogo de corrida em mundo aberto no México.",
                        new List<GameGenre> { GameGenre.Racing, GameGenre.Simulation }
                    ),
                    new Game(
                        "Stardew Valley",
                        14.99m,
                        "Simulador de fazenda e vida no campo.",
                        new List<GameGenre> { GameGenre.Simulation, GameGenre.RPG, GameGenre.Adventure }
                    ),
                    new Game(
                        "Portal 2",
                        9.99m,
                        "Jogo de quebra-cabeças com portais e física.",
                        new List<GameGenre> { GameGenre.Puzzle, GameGenre.Platformer }
                    ),
                    new Game(
                        "The Last of Us Part II",
                        49.99m,
                        "Jogo de ação e aventura pós-apocalíptico.",
                        new List<GameGenre> { GameGenre.Action, GameGenre.Adventure, GameGenre.Horror }
                    ),
                    new Game(
                        "Animal Crossing: New Horizons",
                        54.99m,
                        "Simulador de vida em uma ilha deserta.",
                        new List<GameGenre> { GameGenre.Simulation, GameGenre.Adventure }
                    ),
                    new Game(
                        "Grand Theft Auto V",
                        29.99m,
                        "Mundo aberto com atividades criminosas e exploração.",
                        new List<GameGenre> { GameGenre.Action, GameGenre.Adventure, GameGenre.Sandbox }
                    ),
                    new Game(
                        "Overwatch 2",
                        0m,
                        "Jogo de tiro em primeira pessoa com heróis e habilidades únicas.",
                        new List<GameGenre> { GameGenre.FPS, GameGenre.Action }
                    ),
                    new Game(
                        "Tetris Effect",
                        29.99m,
                        "Versão moderna do clássico jogo de quebra-cabeças.",
                        new List<GameGenre> { GameGenre.Puzzle, GameGenre.Music }
                    ),
                    new Game(
                        "Sekiro: Shadows Die Twice",
                        59.99m,
                        "Jogo de ação com combate intenso e ambientação feudal japonesa.",
                        new List<GameGenre> { GameGenre.Action, GameGenre.Adventure }
                    ),
                    new Game(
                        "Civilization VI",
                        59.99m,
                        "Jogo de estratégia por turnos de construção de impérios.",
                        new List<GameGenre> { GameGenre.Strategy, GameGenre.Simulation }
                    ),
                    new Game(
                        "Rocket League",
                        0m,
                        "Futebol com carros voadores.",
                        new List<GameGenre> { GameGenre.Sports, GameGenre.Racing }
                    ),
                    new Game(
                        "Among Us",
                        4.99m,
                        "Jogo de dedução social em uma nave espacial.",
                        new List<GameGenre> { GameGenre.Puzzle, GameGenre.Social }
                    )
                };

            // Verificar se já existem jogos no banco
            var existingCount = await context.Set<Game>().CountAsync();

            if (existingCount == 0)
            {
                await context.Set<Game>().AddRangeAsync(gamesToSeed);
                await context.SaveChangesAsync();
                Console.WriteLine($"Seeded {gamesToSeed.Count} games successfully!");
            }
            else
            {
                Console.WriteLine($"Database already contains {existingCount} games. Skipping seed.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error on game seed: {ex.Message}");
            Console.WriteLine($"Stack Trace: {ex.StackTrace}");
        }
    }
}