using Domain.Dto;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class GameRepository(SqlContext context) : IGameRepository
    {
        private readonly SqlContext _context = context;

        public async Task<Game> GetGameById(Guid gameId)
        {
            return await _context.Games.FirstOrDefaultAsync(x => x.Id == gameId);
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

        public async Task<IEnumerable<Game>> GetAllGames() => await _context.Games.ToListAsync();
    }
}
