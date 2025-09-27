using Application.DTOs.Requests;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

//retirar
using Domain.Entities;
using Application.DTOs;
using Domain.Enums;
using AutoMapper;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GameElasticSearchController : ControllerBase
    {
        private readonly IGameElasticSearchService _gameElasticSearchService;
        private readonly IMapper _mapper;
        public GameElasticSearchController(IGameElasticSearchService gameElasticSearchService, IMapper mapper)
        {
            _gameElasticSearchService = gameElasticSearchService;
            _mapper = mapper;
        }

        /// <summary>
        /// GET /{id} - Busca um jogo por _id no Elasticsearch.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] string id, CancellationToken ct)
        {
            var game = await _gameElasticSearchService.GetByIdAsync(id, ct);

            if (game is null)
                return NotFound(new { message = "Jogo não encontrado." });

            return Ok(new { id, game });
        }

        /// <summary>
        /// Adciona um jogo ao elastic search
        /// </summary>
        [HttpPost("add-game")]
        public async Task<IActionResult> IndexGame([FromBody] CreateGameDTO dto, CancellationToken ct)
        {
            // Validações de domínio (reuso das regras do Game)
            Game.ValidateTitle(dto.Title);
            Game.ValidatePrice(dto.Price);
            Game.ValidateDescription(dto.Description);
            Game.ValidateGenreList(dto.Genre);

            var game = _mapper.Map<Game>(dto);

            var esId = await _gameElasticSearchService.IndexGameAsync(game, ct);

            return CreatedAtAction(
                actionName: nameof(IndexGame),
                routeValues: new { id = esId },
                value: new { id = esId, indexed = true });
        }

        /// <summary>
        /// APAGA TODOS os documentos do índice configurado (ex.: Search:DefaultIndex).
        /// Use com cuidado! 
        /// </summary>
        [HttpDelete("delete-all-games")]
        public async Task<IActionResult> ClearIndex(CancellationToken ct)
        {
            var deleted = await _gameElasticSearchService.DeleteAllAsync(ct);

            return Ok(new { deleted });
        }

        /// <summary>DELETE /{id} 
        /// - Remove um jogo por _id no Elasticsearch.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteById([FromRoute] string id, CancellationToken ct)
        {
            var ok = await _gameElasticSearchService.DeleteByIdAsync(id, ct);

            if (!ok) 
                return NotFound(new { message = "Jogo não encontrado." });

            return Ok(new { id, deleted = true });
        }
    }
}
