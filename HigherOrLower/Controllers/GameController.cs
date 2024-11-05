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
        public async Task<IActionResult> CreateGame([FromBody] CreateGameDto createGameDto)
        {
            var game = await _gameService.CreateGame(createGameDto);
            return CreatedAtAction(nameof(GetGame), new { game.Id }, game);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetGame(Guid id)
        {
            var game = await _gameService.GetGameById(id);

            if (game == null)
                return NotFound("Game not found.");

            return Ok(new { GameId = id, game.CurrentCard, Players = game.Players.Select(p => new { p.Name, p.Score }) });
        }

        [HttpPost("{gameId}/guess/{playerId}")]
        public async Task<IActionResult> MakeGuess(Guid gameId, Guid playerId, [FromBody] bool guessHigher)
        {
            try
            {
                var (IsCorrect, CurrentCard, GameOver) = await _gameService.MakeGuess(gameId, playerId, guessHigher);
                return Ok(new
                {
                    Correct = IsCorrect,
                    CurrentCard,
                    GameOver
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllGames()
        {
            var games = await _gameService.GetAllGames();
            return Ok(games);
        }

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
