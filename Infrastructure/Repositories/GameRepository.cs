using Domain.Dto;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Contexts;

namespace Infrastructure.Repositories
{
    public class GameRepository(SqlContext context) : IGameRepository
    {
        private readonly SqlContext _context = context;

        public Game GetGameById(Guid gameId)
        {
            return _context.Games.FirstOrDefault(x => x.Id == gameId);
        }

        public async Task<GameDto> AddGameAsync(List<string> playerNames)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            
            var game = new Game();

            if (game.Id == Guid.Empty)
                throw new InvalidOperationException("Game ID was not generated.");

            try
            {
                foreach (var playerName in playerNames)
                    game.AddPlayer(playerName);

                _context.Games.Add(game);

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                var gameDto = new GameDto
                {
                    Id = game.Id,
                    CurrentCard = game.CurrentCard,
                    Players = game.Players.Select(p => new PlayerDto
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Score = p.Score
                    }).ToList()
                };

                return gameDto;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
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
