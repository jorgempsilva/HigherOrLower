using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IGameRepository
    {
        void AddGame(Game game);
        Game GetGameById(Guid gameId);
        IEnumerable<Game> GetAllGames();
    }
}
