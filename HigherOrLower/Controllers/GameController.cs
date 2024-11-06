using Domain.Dto;
using Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace HigherOrLower.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameController(GameService gameService) : ControllerBase
    {
        private readonly GameService _gameService = gameService;

        [HttpPost("newGame")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateGame([FromBody] CreateGameDto createGameDto)
        {
            var game = await _gameService.CreateGame(createGameDto);
            return CreatedAtAction(nameof(GetGame), new { id = game.GameId }, game);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetGame(Guid id)
        {
            var game = await _gameService.GetGameById(id);

            if (game == null)
                return NotFound("Game not found.");

            return Ok(new { GameId = id, Card = game.CurrentCard.Value.ToString() + game.CurrentCard.Suit.ToString(),
                Players = game.Players.Select(x => x.Name ).ToList() });
        }

        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpPost("{gameId}/guess")]
        public async Task<IActionResult> Guess(Guid gameId, [FromQuery] bool guessHigher)
        {
            var result = await _gameService.ProcessGuess(gameId, guessHigher);

            if (result == null)
                return NotFound("Game not found or finished.");

            return Ok(result);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllGames()
        {
            var games = await _gameService.GetAllGames();
            return Ok(games);
        }

        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("{gameId}/score")]
        public async Task<IActionResult> GetScore(Guid gameId)
        {
            try
            {
                var finalScore = await _gameService.GetFinalScore(gameId);
                return Ok(new
                {
                    Player1 = finalScore.ElementAt(0).Name,
                    FinalScore1 = finalScore.ElementAt(0).Score,
                    Player2 = finalScore.ElementAt(1).Name,
                    FinalScore2 = finalScore.ElementAt(1).Score,
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
