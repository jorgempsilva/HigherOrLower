using Domain.Dto;
using Domain.Entities;
using Domain.Interfaces;

namespace Domain.Services
{
    public class GameService(IGameRepository gameRepository)
    {
        private readonly IGameRepository _gameRepository = gameRepository;

        public async Task<GameDto> CreateGame(CreateGameDto createGameDto)
        {
            return await _gameRepository.AddGameAsync(createGameDto.Players);
        }

        public async Task<Game> GetGameById(Guid gameId)
        {
            return await _gameRepository.GetGameById(gameId);
        }

        public async Task<(bool IsCorrect, string CurrentCard, bool GameOver)> MakeGuess(Guid gameId, Guid playerId, bool guessHigher)
        {
            var game = await _gameRepository.GetGameById(gameId) ?? throw new Exception("Game not found.");

            if (game.IsGameOver) throw new Exception("Game is over.");

            bool isCorrect = game.MakeGuess(game.Id, playerId, guessHigher);

            return (isCorrect, game.CurrentCard, game.IsGameOver);
        }

        public async Task<IEnumerable<Game>> GetAllGames()
        {
            var games = await _gameRepository.GetAllGames();
            return games;
        }

        public async Task<List<Player>> GetFinalScore(Guid gameId)
        {
            var game = await _gameRepository.GetGameById(gameId) ?? throw new Exception("Game not found.");
            
            if (!game.IsGameOver) throw new Exception("Game is not yet over.");

            return game.Players;
        }
    }
}
