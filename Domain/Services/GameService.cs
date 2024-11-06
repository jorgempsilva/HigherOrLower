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

        public async Task<GameDto> ProcessGuess(Guid gameId, bool guess)
        {
            var game = await _gameRepository.GetGameById(gameId) ?? throw new Exception("Game not found");

            if (game.Deck.Count < 2)
                throw new Exception("No more cards in the deck to continue.");

            var nextCard = game.Deck[1];

            return await _gameRepository.MakeGuess(guess, game, nextCard);
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
