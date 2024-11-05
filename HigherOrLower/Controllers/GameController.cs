using Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace HigherOrLower.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameController(GameService gameService) : ControllerBase
    {
        private readonly GameService _gameService = gameService;
        private static readonly Dictionary<Guid, GameService> Games = new();

        [HttpPost("new")]
        public async Task<IActionResult> CreateGame(string nome1, string nome2)
        {
            var game = await _gameService.CreateGame(nome1, nome2);
            return CreatedAtAction(nameof(GetGame), new { game.Id }, game);
        }

        [HttpGet("{id}")]
        public IActionResult GetGame(Guid id)
        {
            try
            {
                var game = _gameService.GetGameById(id);

                if (game == null)
                    return NotFound("Game not found.");

                return Ok(new { GameId = id, game.CurrentCard, Players = game.Players.Select(p => new { p.Name, p.Score }) });
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost("{gameId}/guess/{playerId}")]
        public IActionResult MakeGuess(Guid gameId, Guid playerId, [FromBody] bool guessHigher)
        {
            try
            {
                var (IsCorrect, CurrentCard, GameOver) = _gameService.MakeGuess(gameId, playerId, guessHigher);
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
        public IActionResult GetAllGames()
        {
            var games = _gameService.GetAllGames();
            return Ok(games);
        }

        [HttpGet("{gameId}/score")]
        public IActionResult GetScore(Guid gameId)
        {
            try
            {
                var finalScore = _gameService.GetFinalScore(gameId);
                return Ok(new {
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
