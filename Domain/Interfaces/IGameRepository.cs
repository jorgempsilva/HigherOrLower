using Domain.Dto;
using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IGameRepository
    {
        Task<GameDto> AddGameAsync(List<string> playerNames);
        Task<GameDto> MakeGuess(bool guess, Game game, Card nextCard);
        Task<Game> GetGameById(Guid gameId);
        Task<IEnumerable<Game>> GetAllGames();
    }
}
