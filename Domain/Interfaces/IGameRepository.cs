using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IGameRepository
    {
        void AddGame(Game game);
        void AddPlayer(Player player);
        Game GetGameById(Guid gameId);
        IEnumerable<Game> GetAllGames();
    }
}
