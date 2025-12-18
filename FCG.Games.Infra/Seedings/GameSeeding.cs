using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.IndexManagement;
using FCG.Games.Domain.Entities;
using FCG.Games.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace FCG.Games.Infra.Seedings;

public class GameSeeding
{
    public static async Task SeedAsync(ElasticsearchClient client, IConfiguration configuration)
    {
        try
        {
            Console.WriteLine("Starting game seed...");

            var indexName = configuration["ElasticSearch:IndexName"] ?? "games";

            var gamesToSeed = new List<Game>()
                {
                    Game.Create(
                        "The Legend of Zelda: Breath of the Wild",
                        59.99m,
                        "Um jogo de aventura em mundo aberto onde você explora o reino de Hyrule.",
                        [GameGenre.Adventure, GameGenre.Action, GameGenre.RPG]
                    ),
                    Game.Create(
                        "Super Mario Odyssey",
                        49.99m,
                        "Uma aventura 3D com Mario em diversos mundos para resgatar a Princesa Peach.",
                        [GameGenre.Platformer, GameGenre.Adventure]
                    ),
                    Game.Create(
                        "Minecraft",
                        26.95m,
                        "Um jogo de construção e sobrevivência em um mundo gerado aleatoriamente.",
                        [GameGenre.Sandbox, GameGenre.Survival, GameGenre.Adventure]
                    ),
                    Game.Create(
                        "The Witcher 3: Wild Hunt",
                        39.99m,
                        "Um RPG de ação em mundo aberto onde você joga como Geralt de Rivia em busca de sua filha adotiva.",
                        [GameGenre.RPG, GameGenre.Action, GameGenre.Adventure]
                    ),
                    Game.Create(
                        "Dark Souls III",
                        29.99m,
                        "Um RPG de ação desafiador com combate intenso e exploração em um mundo sombrio.",
                        [GameGenre.RPG, GameGenre.Action, GameGenre.Horror]
                    ),
                    Game.Create(
                        "God of War 3",
                        39.99m,
                        "Uma reinvenção da série God of War, focada na relação entre Kratos e seu filho Atreus.",
                        [GameGenre.Action, GameGenre.Adventure, GameGenre.RPG] 
                    ),
                    Game.Create(
                        "Hollow Knight",
                        14.99m,
                        "Um jogo de plataforma e ação em um mundo subterrâneo cheio de segredos e desafios.",
                        [GameGenre.Platformer, GameGenre.Action, GameGenre.Adventure]
                    ),
                    Game.Create(
                        "Red Dead Redemption 2",
                        59.99m,
                        "Um jogo de ação e aventura no velho oeste americano.",
                        [GameGenre.Action, GameGenre.Adventure, GameGenre.Sandbox]
                    ),
                    Game.Create(
                        "Cyberpunk 2077",
                        49.99m,
                        "Um RPG de ação em um futuro distópico cyberpunk.",
                        [GameGenre.RPG, GameGenre.Action, GameGenre.Adventure]
                    ),
                    Game.Create(
                        "Elden Ring",
                        59.99m,
                        "Um RPG de ação em mundo aberto desenvolvido pela FromSoftware.",
                        [GameGenre.RPG, GameGenre.Action, GameGenre.Adventure]
                    ),
                    Game.Create(
                        "FIFA 23",
                        59.99m,
                        "Simulador de futebol com times e jogadores reais.",
                        [GameGenre.Sports, GameGenre.Simulation]
                    ),
                    Game.Create(
                        "Call of Duty: Modern Warfare II",
                        69.99m,
                        "Jogo de tiro em primeira pessoa com campanha e multiplayer.",
                        [GameGenre.FPS, GameGenre.Action]
                    ),
                    Game.Create(
                        "Resident Evil 4 Remake",
                        59.99m,
                        "Remake do clássico jogo de survival horror.",
                        [GameGenre.Horror, GameGenre.Action, GameGenre.Adventure]
                    ),
                    Game.Create(
                        "Forza Horizon 5",
                        59.99m,
                        "Jogo de corrida em mundo aberto no México.",
                        [GameGenre.Racing, GameGenre.Simulation]
                    ),
                    Game.Create(
                        "Stardew Valley",
                        14.99m,
                        "Simulador de fazenda e vida no campo.",
                        [GameGenre.Simulation, GameGenre.RPG, GameGenre.Adventure]
                    ),
                    Game.Create(
                        "Portal 2",
                        9.99m,
                        "Jogo de quebra-cabeças com portais e física.",
                        [GameGenre.Puzzle, GameGenre.Platformer]
                    ),
                    Game.Create(
                        "The Last of Us Part II",
                        49.99m,
                        "Jogo de ação e aventura pós-apocalíptico.",
                        [GameGenre.Action, GameGenre.Adventure, GameGenre.Horror]
                    ),
                    Game.Create(
                        "Animal Crossing: New Horizons",
                        54.99m,
                        "Simulador de vida em uma ilha deserta.",
                        [GameGenre.Simulation, GameGenre.Adventure]
                    ),
                    Game.Create(
                        "Grand Theft Auto V",
                        29.99m,
                        "Mundo aberto com atividades criminosas e exploração.",
                        [GameGenre.Action, GameGenre.Adventure, GameGenre.Sandbox]
                    ),
                    Game.Create(
                        "Overwatch 2",
                        0m,
                        "Jogo de tiro em primeira pessoa com heróis e habilidades únicas.",
                        [GameGenre.FPS, GameGenre.Action]
                    ),
                    Game.Create(
                        "Tetris Effect",
                        29.99m,
                        "Versão moderna do clássico jogo de quebra-cabeças.",
                        [GameGenre.Puzzle, GameGenre.Music]
                    ),
                    Game.Create(
                        "Sekiro: Shadows Die Twice",
                        59.99m,
                        "Jogo de ação com combate intenso e ambientação feudal japonesa.",
                        [GameGenre.Action, GameGenre.Adventure]
                    ),
                    Game.Create(
                        "Civilization VI",
                        59.99m,
                        "Jogo de estratégia por turnos de construção de impérios.",
                        [GameGenre.Strategy, GameGenre.Simulation]
                    ),
                    Game.Create(
                        "Rocket League",
                        0m,
                        "Futebol com carros voadores.",
                        [GameGenre.Sports, GameGenre.Racing]
                    ),
                    Game.Create(
                        "Among Us",
                        4.99m,
                        "Jogo de dedução social em uma nave espacial.",
                        [GameGenre.Puzzle, GameGenre.Social]
                    )
                };

            // Verificar se já existem jogos no banco
            var existsResponse = await client.Indices.ExistsAsync(indexName);

            if (!existsResponse.Exists)
            {
                Console.WriteLine($"Index '{indexName}' não existe. Processo falhou.");
                return;
            }

            var countResponse = await client.CountAsync<Game>(c => c.Indices(indexName));

            if (!countResponse.IsValidResponse)
            {
                Console.WriteLine("Não foi possível verificar o número de documentos. Processando inicialização de jogos...");
            }
            else if (countResponse.Count > 0)
            {
                Console.WriteLine($"Index já contém {countResponse.Count} jogos. Inicialização ed jogos cancelada.");
                return;
            }

            // Inserir documentos em lote usando BulkAsync
            var bulkResponse = await client.BulkAsync(b => b
                .Index(indexName)
                .IndexMany(gamesToSeed)
            );

            if (!bulkResponse.IsValidResponse)
            {
                Console.WriteLine($"Erro durante a inserção em lote: {bulkResponse.DebugInformation}");
                throw new Exception($"Inicialização de jogos falhou: {bulkResponse.DebugInformation}");
            }

            if (bulkResponse.Errors)
            {
                var failedDocuments = bulkResponse.ItemsWithErrors;
                Console.WriteLine($"Alguns documentos falharam ao ser indexados. Número de falhas: {failedDocuments.Count()}");
                foreach (var item in failedDocuments)
                {
                    Console.WriteLine($"Falha do documento: {item.Error?.Reason}");
                }
            }
            else
            {
                Console.WriteLine($"Inicialização de {gamesToSeed.Count} jogos com sucesso!");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro na inicialização: {ex.Message}");
            Console.WriteLine($"Stack Trace: {ex.StackTrace}");
        }
    }
}
