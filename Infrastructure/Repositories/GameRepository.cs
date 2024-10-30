using Domain.Entities;
using Domain.Interfaces;

namespace Infrastructure.Repositories
{
    public class GameRepository : IGameRepository
    {
        private readonly Dictionary<Guid, Game> _games = [];

        public Game GetGameById(Guid gameId)
        {
            _games.TryGetValue(gameId, out var game);
            return game;
        }

        public void AddGame(Game game)
        {
            _games.Add(game.Id, game);
        }

        IEnumerable<Game> IGameRepository.GetAllGames()
        {
            var games = new List<Game>();
            foreach (var game in _games)
                games.Add(game.Value);

            return games;
        }
    }
}
