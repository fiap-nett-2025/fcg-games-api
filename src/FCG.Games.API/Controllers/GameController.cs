using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FCG.Games.Application.DTOs;
using FCG.Games.Application.Interfaces;

namespace FCG.Games.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    
    public class GameController(IGameService gameService) : ApiBaseController
    {
        [HttpGet()]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int size = 10)
        {
             if (page < 1)
             {
                return BadRequest("O número da página deve ser maior que 0.");
             }
            var games = await gameService.GetGamesPaginatedAsync(page, size);
            return Success(games);
        }

        [HttpGet("Popularity")]
        public async Task<IActionResult> GetPopularGames([FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            if (page < 1)
            {
                return BadRequest("O número da página deve ser maior que 0.");
            }
            var games = await gameService.GetMostPopularGamesPaginatedAsync(page, size);
            return Success(games);
        }

        [HttpGet("Recommended")]
        public async Task<IActionResult> GetRecommendedGames([FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            if (page < 1)
            {
                return BadRequest("O número da página deve ser maior que 0.");
            }

            var userId = GetUserId();
            var jwt = GetJwtToken();

            var games = await gameService.GetRecommendedGamesPaginatedAsync(page, size, userId, jwt);
            return Success(games);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] string id)
        {
            var game = await gameService.GetByIdAsync(id);
            return Success(game, "Jogo encontrado.");
        }

        [HttpPatch("Popularity")]
        public async Task<IActionResult> IncreasePopularity([FromBody] List<string> ids)
        {
            var games = await gameService.IncreasePopularityAsync(ids);
            return Success(games, "Popularidade atualizada com sucesso.");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> InsertGame([FromBody] CreateGameDTO dto)
        {
            var game = await gameService.InsertAsync(dto);
            return CreatedResponse(game, "Jogo criado com sucesso.");
        }

        [HttpPatch("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateGame([FromRoute] string id, [FromBody] PartialUpdateGameDTO dto)
        {
            var game = await gameService.PartialUpdateAsync(id, dto);
            return Success(game, "Jogo atualizado com sucesso.");
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteById([FromRoute] string id)
        {
            await gameService.DeleteByIdAsync(id);
            return NoContent();
        }

        /// <summary>
        /// APAGA TODOS os documentos do índice configurado (ex.: Search:DefaultIndex).
        /// Use com cuidado! 
        /// </summary>
        [HttpDelete("DeleteAll")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ClearIndex()
        {
            await gameService.DeleteAllAsync();
            return NoContent();
        }
    }
}
