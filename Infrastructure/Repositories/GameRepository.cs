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
            return await _context.Games.Include(g => g.Players).FirstOrDefaultAsync(x => x.Id == gameId);
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
                    GameId = game.Id,
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

        public async Task<GameDto> MakeGuess(bool guess, Game game, Card nextCard)
        {
            var isCorrectGuess =
                            (guess == true && nextCard.Value >= game.CurrentCard.Value) ||
                            (guess == false && nextCard.Value <= game.CurrentCard.Value);

            if (isCorrectGuess)
                game.Players[game.CurrentPlayerIndex].Score += 1;

            game.Deck.RemoveAt(0);
            game.CurrentCard = nextCard;

            game.CurrentPlayerIndex = (game.CurrentPlayerIndex + 1) % game.Players.Count;
            game.Players.ForEach(p => p.IsTurn = false);
            game.Players[game.CurrentPlayerIndex].IsTurn = true;

            await _context.SaveChangesAsync();

            return new GameDto
            {
                GameId = game.Id,
                CurrentCard = game.CurrentCard,
                Players = game.Players.Select(p => new PlayerDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Score = p.Score
                }).ToList()
            };
        }

        public async Task<IEnumerable<Game>> GetAllGames() => await _context.Games.Include(g => g.Players).ToListAsync();
    }
}
