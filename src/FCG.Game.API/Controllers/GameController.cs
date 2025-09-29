using Microsoft.AspNetCore.Mvc;
using FCG.Game.Application.Interfaces;
using FCG.Game.Application.DTOs;

namespace FCG.Game.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GameController(IGameService gameService) : ApiBaseController
    {

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] string id)
        {
            var game = await gameService.GetByIdAsync(id);
            return Success(game, "Jogo encontrado.");
        }

        [HttpPatch("Popularity/{id}")]
        public async Task<IActionResult> IncreasePopularity([FromRoute] string id)
        {
            var game = await gameService.IncreasePopularity(id);
            return Success(game, "Popularidade atualizada com sucesso.");
        }

        [HttpPost]
        public async Task<IActionResult> InsertGame([FromBody] CreateGameDTO dto)
        {
            var game = await gameService.InsertAsync(dto);
            return CreatedResponse(game, "Jogo criado com sucesso.");
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateGame([FromRoute] string id, [FromBody] PartialUpdateGameDTO dto)
        {
            var game = await gameService.PartialUpdateAsync(id, dto);
            return Success(game, "Jogo atualizado com sucesso.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteById([FromRoute] string id)
        {
            await gameService.DeleteByIdAsync(id);
            return NoContent();
        }

        /// <summary>
        /// APAGA TODOS os documentos do índice configurado (ex.: Search:DefaultIndex).
        /// Use com cuidado! 
        /// </summary>
        [HttpDelete("delete-all-games")]
        public async Task<IActionResult> ClearIndex()
        {
            await gameService.DeleteAllAsync();
            return NoContent();
        }
    }
}
