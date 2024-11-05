using Domain.Dto;
using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IGameRepository
    {
        Task<GameDto> AddGameAsync(List<string> playerNames);
        void AddPlayer(Player player);
        Task<Game> GetGameById(Guid gameId);
        Task<IEnumerable<Game>> GetAllGames();
    }
}
