﻿using Domain.Dto;
using Domain.Entities;
using Domain.Interfaces;

namespace Domain.Services
{
    public class GameService(IGameRepository gameRepository)
    {
        private readonly IGameRepository _gameRepository = gameRepository;

        public async Task<GameDto> CreateGame(string nome1, string nome2)
        {
            return await _gameRepository.AddGameAsync([nome1, nome2]);
        }

        public Game GetGameById(Guid gameId)
        {
            return _gameRepository.GetGameById(gameId);
        }

        public (bool IsCorrect, string CurrentCard, bool GameOver) MakeGuess(Guid gameId, Guid playerId, bool guessHigher)
        {
            var game = _gameRepository.GetGameById(gameId) ?? throw new Exception("Game not found.");

            if (game.IsGameOver) throw new Exception("Game is over.");

            bool isCorrect = game.MakeGuess(game.Id, playerId, guessHigher);

            return (isCorrect, game.CurrentCard, game.IsGameOver);
        }

        public IEnumerable<Game> GetAllGames()
        {
            var games = _gameRepository.GetAllGames();
            return games;
        }

        public List<Player> GetFinalScore(Guid gameId)
        {
            var game = _gameRepository.GetGameById(gameId) ?? throw new Exception("Game not found.");
            
            if (!game.IsGameOver) throw new Exception("Game is not yet over.");

            return game.Players;
        }
    }
}
