using FCG.Games.Domain.Entities;
using FCG.Games.Domain.Enums;
using FCG.Games.Infra.Data.Data;
using Microsoft.EntityFrameworkCore;

namespace FCG.Games.Infra.Data.Seedings;

public class PromotionSeeding
{
    public static async Task SeedAsync(FcgGameDbContext context)
    {
		try
		{
            Console.WriteLine("Iniciando catálogo de promoções...");

            var currentYear = DateTime.UtcNow.Year;
            var currentMonth = DateTime.UtcNow.Month;
            var promotionsToSeed = new List<Promotion>
            {
                // ========== PROMOÇÕES SAZONAIS - ANO CORRENTE ==========
                
                // Janeiro - Réveillon e Volta às Aulas
                new Promotion(
                    "🎆 Réveillon dos Games",
                    "Comece o ano com descontos explosivos em jogos de ação!",
                    35,
                    GameGenre.Action,
                    new DateTime(currentYear, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    new DateTime(currentYear, 1, 7, 23, 59, 59, DateTimeKind.Utc)
                ),

                new Promotion(
                    "📚 Volta às Aulas Educativa",
                    "Aprenda enquanto se diverte! Jogos educativos em promoção.",
                    20,
                    GameGenre.Educational,
                    new DateTime(currentYear, 1, 20, 0, 0, 0, DateTimeKind.Utc),
                    new DateTime(currentYear, 2, 15, 23, 59, 59, DateTimeKind.Utc)
                ),
                
                // Fevereiro - Carnaval
                new Promotion(
                    "🎭 Carnaval Musical",
                    "Caia na folia com jogos de música e ritmo!",
                    25,
                    GameGenre.Music,
                    new DateTime(currentYear, 2, 10, 0, 0, 0, DateTimeKind.Utc),
                    new DateTime(currentYear, 2, 28, 23, 59, 59, DateTimeKind.Utc)
                ),
                
                // Março - Dia Internacional da Mulher
                new Promotion(
                    "💪 Heroínas em Ação",
                    "Celebrando personagens femininas fortes! Jogos de ação e aventura.",
                    30,
                    GameGenre.Adventure,
                    new DateTime(currentYear, 3, 1, 0, 0, 0, DateTimeKind.Utc),
                    new DateTime(currentYear, 3, 10, 23, 59, 59, DateTimeKind.Utc)
                ),
                
                // Abril - Páscoa e Dia da Mentira
                new Promotion(
                    "🐰 Páscoa dos Plataformas",
                    "Pule de alegria com jogos de plataforma em promoção!",
                    28,
                    GameGenre.Platformer,
                    new DateTime(currentYear, 4, 10, 0, 0, 0, DateTimeKind.Utc),
                    new DateTime(currentYear, 4, 25, 23, 59, 59, DateTimeKind.Utc)
                ),

                new Promotion(
                    "🃏 Festival dos Puzzles",
                    "Desafie sua mente com jogos de quebra-cabeça!",
                    22,
                    GameGenre.Puzzle,
                    new DateTime(currentYear, 4, 1, 0, 0, 0, DateTimeKind.Utc),
                    new DateTime(currentYear, 4, 7, 23, 59, 59, DateTimeKind.Utc)
                ),
                
                // Maio - Dia das Mães
                new Promotion(
                    "💐 Jogos para Toda Família",
                    "Celebre com jogos que toda a família pode curtir!",
                    25,
                    GameGenre.Social,
                    new DateTime(currentYear, 5, 1, 0, 0, 0, DateTimeKind.Utc),
                    new DateTime(currentYear, 5, 15, 23, 59, 59, DateTimeKind.Utc)
                ),
                
                // Junho - Festa Junina e Dia dos Namorados
                new Promotion(
                    "🌽 Arraiá dos Games",
                    "Jogos de música e festa! Celebre as festas juninas jogando.",
                    20,
                    GameGenre.Music,
                    new DateTime(currentYear, 6, 10, 0, 0, 0, DateTimeKind.Utc),
                    new DateTime(currentYear, 6, 30, 23, 59, 59, DateTimeKind.Utc)
                ),

                new Promotion(
                    "💘 Games para Casais",
                    "Jogue junto com quem você ama! Puzzles cooperativos em promoção.",
                    18,
                    GameGenre.Puzzle,
                    new DateTime(currentYear, 6, 5, 0, 0, 0, DateTimeKind.Utc),
                    new DateTime(currentYear, 6, 15, 23, 59, 59, DateTimeKind.Utc)
                ),
                
                // Julho - Férias de Inverno
                new Promotion(
                    "❄️ Férias de Inverno - Aventuras",
                    "Explore novos mundos nas férias! Jogos de aventura em promoção.",
                    30,
                    GameGenre.Adventure,
                    new DateTime(currentYear, 7, 1, 0, 0, 0, DateTimeKind.Utc),
                    new DateTime(currentYear, 7, 31, 23, 59, 59, DateTimeKind.Utc)
                ),

                new Promotion(
                    "🎮 Férias Gamer - Sandbox",
                    "Construa seu próprio mundo durante as férias!",
                    35,
                    GameGenre.Sandbox,
                    new DateTime(currentYear, 7, 10, 0, 0, 0, DateTimeKind.Utc),
                    new DateTime(currentYear, 7, 25, 23, 59, 59, DateTimeKind.Utc)
                ),
                
                // Agosto - Dia dos Pais
                new Promotion(
                    "🥊 Torneio dos Campeões - Lutas",
                    "Para os pais guerreiros! Jogos de luta em promoção.",
                    25,
                    GameGenre.Fighting,
                    new DateTime(currentYear, 8, 1, 0, 0, 0, DateTimeKind.Utc),
                    new DateTime(currentYear, 8, 15, 23, 59, 59, DateTimeKind.Utc)
                ),

                new Promotion(
                    "🧠 Mestres da Estratégia",
                    "Celebre com jogos que exigem raciocínio e planejamento!",
                    28,
                    GameGenre.Strategy,
                    new DateTime(currentYear, 8, 5, 0, 0, 0, DateTimeKind.Utc),
                    new DateTime(currentYear, 8, 20, 23, 59, 59, DateTimeKind.Utc)
                ),
                
                // Setembro - Primavera
                new Promotion(
                    "🌸 Primavera dos Simuladores",
                    "Renasça com jogos de simulação! Crie sua própria realidade.",
                    22,
                    GameGenre.Simulation,
                    new DateTime(currentYear, 9, 21, 0, 0, 0, DateTimeKind.Utc),
                    new DateTime(currentYear, 10, 10, 23, 59, 59, DateTimeKind.Utc)
                ),
                
                // Outubro - Dia das Crianças e Halloween
                new Promotion(
                    "🎈 Especial Dia das Crianças",
                    "Diversão garantida com jogos de plataforma!",
                    30,
                    GameGenre.Platformer,
                    new DateTime(currentYear, 10, 5, 0, 0, 0, DateTimeKind.Utc),
                    new DateTime(currentYear, 10, 15, 23, 59, 59, DateTimeKind.Utc)
                ),

                new Promotion(
                    "🎃 Halloween Horror Fest",
                    "Prepare-se para sustos! Jogos de terror com desconto assustador.",
                    35,
                    GameGenre.Horror,
                    new DateTime(currentYear, 10, 20, 0, 0, 0, DateTimeKind.Utc),
                    new DateTime(currentYear, 11, 2, 23, 59, 59, DateTimeKind.Utc)
                ),
                
                // Novembro - Black Friday
                new Promotion(
                    "🛒 Black Friday - FPS Explosivo",
                    "A maior promoção do ano! Jogos de tiro com até 50% OFF.",
                    50,
                    GameGenre.FPS,
                    new DateTime(currentYear, 11, 25, 0, 0, 0, DateTimeKind.Utc),
                    new DateTime(currentYear, 11, 29, 23, 59, 59, DateTimeKind.Utc)
                ),

                new Promotion(
                    "🏁 Black Friday Racing",
                    "Acelere nas ofertas! Jogos de corrida com desconto máximo.",
                    45,
                    GameGenre.Racing,
                    new DateTime(currentYear, 11, 25, 0, 0, 0, DateTimeKind.Utc),
                    new DateTime(currentYear, 11, 29, 23, 59, 59, DateTimeKind.Utc)
                ),

                new Promotion(
                    "⚔️ Black Friday RPG Épico",
                    "Aventuras épicas com preços históricos!",
                    48,
                    GameGenre.RPG,
                    new DateTime(currentYear, 11, 25, 0, 0, 0, DateTimeKind.Utc),
                    new DateTime(currentYear, 11, 30, 23, 59, 59, DateTimeKind.Utc)
                ),
                
                // Dezembro - Natal e Final de Ano
                new Promotion(
                    "🎄 Natal dos Aventureiros",
                    "Explore novos mundos neste Natal! Jogos de aventura em promoção.",
                    40,
                    GameGenre.Adventure,
                    new DateTime(currentYear, 12, 10, 0, 0, 0, DateTimeKind.Utc),
                    new DateTime(currentYear, 12, 26, 23, 59, 59, DateTimeKind.Utc)
                ),

                new Promotion(
                    "🎅 Natal Mágico - Sandbox",
                    "Construa seu próprio Natal! Jogos sandbox com até 40% OFF.",
                    40,
                    GameGenre.Sandbox,
                    new DateTime(currentYear, 12, 15, 0, 0, 0, DateTimeKind.Utc),
                    new DateTime(currentYear, 12, 31, 23, 59, 59, DateTimeKind.Utc)
                ),

                new Promotion(
                    "⚽ Reveillon Esportivo",
                    "Entre em campo no ano novo! Jogos de esporte em promoção.",
                    35,
                    GameGenre.Sports,
                    new DateTime(currentYear, 12, 26, 0, 0, 0, DateTimeKind.Utc),
                    new DateTime(currentYear, 12, 31, 23, 59, 59, DateTimeKind.Utc)
                ),

                new Promotion(
                    "🎊 Reveillon Survival",
                    "Sobreviva ao ano novo! Jogos de sobrevivência com desconto.",
                    38,
                    GameGenre.Survival,
                    new DateTime(currentYear, 12, 28, 0, 0, 0, DateTimeKind.Utc),
                    new DateTime(currentYear + 1, 1, 5, 23, 59, 59, DateTimeKind.Utc)
                ),
                
                // ========== PROMOÇÕES DO ANO SEGUINTE (FUTURAS) ==========
                
                // Ano Novo Seguinte
                new Promotion(
                    "🎆 Ano Novo - Novos Mundos",
                    "Comece o novo ano explorando novos mundos! Ação e aventura.",
                    40,
                    GameGenre.Action,
                    new DateTime(currentYear + 1, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    new DateTime(currentYear + 1, 1, 10, 23, 59, 59, DateTimeKind.Utc)
                ),

                new Promotion(
                    "📚 Volta às Aulas - Próximo Ano",
                    "Prepare-se para aprender jogando no novo ano letivo!",
                    25,
                    GameGenre.Educational,
                    new DateTime(currentYear + 1, 1, 25, 0, 0, 0, DateTimeKind.Utc),
                    new DateTime(currentYear + 1, 2, 20, 23, 59, 59, DateTimeKind.Utc)
                ),
            };

            var existingCount = await context.Set<Promotion>().CountAsync();

            if(existingCount == 0)
            {
                await context.Set<Promotion>().AddRangeAsync(promotionsToSeed);
                await context.SaveChangesAsync();
                Console.WriteLine("Catálogo inicial de promoções concluído com sucesso.");
            }
            else
            {
                Console.WriteLine($"Catálogo de promoções já existe com {existingCount} promoções. Nenhuma ação foi tomada.");
            }
        }
		catch (Exception ex)
		{
            Console.WriteLine($"Erro na criação do catálogo: {ex.Message}");
            Console.WriteLine($"Stack Trace: {ex.StackTrace}");
        }
    }
}
