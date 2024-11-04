using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Contexts;

namespace Infrastructure.Repositories
{
    public class GameRepository : IGameRepository
    {
        private readonly SqlContext _context;

        public GameRepository(SqlContext context)
        {
            _context = context;
        }

        public Game GetGameById(Guid gameId)
        {
            return _context.Games.FirstOrDefault(x => x.Id == gameId);
        }

        public void AddGame(Game game)
        {
            _context.Games.Add(game);
            _context.SaveChanges();
        }

        public void AddPlayer(Player player)
        {
            _context.Players.Add(player);
            _context.SaveChanges();
        }

        IEnumerable<Game> IGameRepository.GetAllGames()
        {
            return [.. _context.Games];
        }
    }
}
