using Domain.Dto;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Services;
using FluentAssertions;
using HigherOrLower.Controllers;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Text.Json;

namespace Tests.Unit.Api.Controllers
{
    public class GameControllerTest
    {
        private readonly GameController _controller;
        private readonly GameService _gameService;
        private readonly Mock<IGameRepository> _repositoryMock;
        public GameControllerTest()
        {
            _repositoryMock = new Mock<IGameRepository>();
            _gameService = new GameService(_repositoryMock.Object);
            _controller = new GameController(_gameService);
        }

        [Fact]
        public async Task CreateGame_WithEmptyList_ReturnInvalidOperationException()
        {
            // Arrange
            var createGameDto = new CreateGameDto() { Players = [] };
            var gameDto = GenerateGameDto();

            _repositoryMock.Setup(x => x.AddGameAsync(createGameDto.Players)).Throws<InvalidOperationException>();

            // Act
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () => await _gameService.CreateGame(createGameDto));

            // Assert
            ex.Should().BeOfType<InvalidOperationException>();
            ex.Message.Should().Be("Operation is not valid due to the current state of the object.");
        }

        [Fact]
        public async Task CreateGame_With2Players_ReturnCreated()
        {
            // Arrange
            var createGameDto = new CreateGameDto() { Players = ["ze", "manel"] };
            var gameDto = GenerateGameDto();
            _repositoryMock.Setup(x => x.AddGameAsync(createGameDto.Players)).ReturnsAsync(gameDto);

            // Act
            var response = await _controller.CreateGame(createGameDto);

            // Assert
            response.Should().BeOfType<CreatedAtActionResult>();
            ((CreatedAtActionResult)response).Value.Should().Be(gameDto);
        }

        [Fact]
        public async Task GetGame_ValidId_ReturnOk()
        {
            // Arrange
            var deckJson = "[{\"Id\":\"226b70f2-0231-4457-9615-d00a56db8f84\",\"Value\":1,\"Suit\":0}, {\"Id\":\"f0b3f62e-d827-48a7-87b5-78590c9fe04b\",\"Value\":2,\"Suit\":1}]";
            var game = new Game() 
            { 
                Deck = [ new Card(CardValue.Ace,CardSuit.Spades)],
                DeckJson = deckJson
            };
            _repositoryMock.Setup(x => x.GetGameById(game.Id)).ReturnsAsync(game);

            // Act
            var response = await _controller.GetGame(game.Id);

            // Assert
            response.Should().BeOfType<OkObjectResult>();
            var result = new
            {
                GameId = game.Id,
                Card = game.Deck[0].Value.ToString() + " of " + game.Deck[0].Suit.ToString(),
                Players = new List<Player>(),
            };
            var expectedJson = JsonSerializer.Serialize(result);
            var actualJson = JsonSerializer.Serialize(((OkObjectResult)response).Value);

            expectedJson.Should().Be(actualJson);
        }

        [Fact]
        public async Task GetGame_WithNoDeck_ReturnArgumentNullException()
        {
            // Arrange
            var game = new Game();
            _repositoryMock.Setup(x => x.GetGameById(game.Id)).ReturnsAsync(game);

            // Act
            var ex = await Assert.ThrowsAsync<ArgumentNullException>(async () => await _controller.GetGame(game.Id));

            // Assert
            ex.Should().BeOfType<ArgumentNullException>();
            ex.Message.Should().Be("Value cannot be null. (Parameter 'json')");
        }

        [Fact]
        public async Task GetGame_GameNull_ReturnNotFound()
        {
            // Arrange
            Game game = null;
            _repositoryMock.Setup(x => x.GetGameById(Guid.NewGuid())).ReturnsAsync(game);

            // Act
            var response = await _controller.GetGame(Guid.NewGuid());

            // Assert
            response.Should().BeOfType<NotFoundObjectResult>();
            ((NotFoundObjectResult)response).Value.Should().Be("Game not found.");
        }

        [Fact]
        public async Task Guess_GameDtoNull_ReturnNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();
            GameDto gameDto = null;
            var deckJson = "[{\"Id\":\"226b70f2-0231-4457-9615-d00a56db8f84\",\"Value\":1,\"Suit\":0}, {\"Id\":\"f0b3f62e-d827-48a7-87b5-78590c9fe04b\",\"Value\":2,\"Suit\":1}]";
            var game = new Game()
            {
                Deck = [new Card(CardValue.Ace, CardSuit.Spades), new Card(CardValue.Nine, CardSuit.Diamonds), new Card(CardValue.Nine, CardSuit.Spades)],
                DeckJson = deckJson
            };
            _repositoryMock.Setup(x => x.GetGameById(id)).ReturnsAsync(game);
            _repositoryMock.Setup(x => x.MakeGuess(It.IsAny<bool>(), It.IsAny<Game>(), It.IsAny<Card>()))
                   .ReturnsAsync(gameDto);

            // Act
            var response = await _controller.Guess(id, true);

            // Assert
            response.Should().BeOfType<NotFoundObjectResult>();
            ((NotFoundObjectResult)response).Value.Should().Be("Game not found or finished.");
        }

        [Fact]
        public async Task Guess_GameNull_ReturnBadRequest()
        {
            // Act
            var ex = await Assert.ThrowsAsync<Exception>(async () => await _gameService.ProcessGuess(Guid.NewGuid(), true));

            // Assert
            ex.Should().BeOfType<Exception>();
            ex.Message.Should().Be("Game not found");
        }

        [Fact]
        public async Task Guess_GameDtoNull_ReturnOk()
        {
            // Arrange
            var id = Guid.NewGuid();
            GameDto gameDto = GenerateGameDto();
            var deckJson = "[{\"Id\":\"226b70f2-0231-4457-9615-d00a56db8f84\",\"Value\":1,\"Suit\":0}, {\"Id\":\"f0b3f62e-d827-48a7-87b5-78590c9fe04b\",\"Value\":2,\"Suit\":1}]";
            var game = new Game()
            {
                Deck = [new Card(CardValue.Ace, CardSuit.Spades), new Card(CardValue.Nine, CardSuit.Diamonds), new Card(CardValue.Nine, CardSuit.Spades)],
                DeckJson = deckJson
            };
            var card = new Card(CardValue.Nine, CardSuit.Diamonds);
            _repositoryMock.Setup(x => x.GetGameById(id)).ReturnsAsync(game);
            _repositoryMock.Setup(x => x.MakeGuess(It.IsAny<bool>(), It.IsAny<Game>(), It.IsAny<Card>())).ReturnsAsync(gameDto);

            // Act
            var response = await _controller.Guess(id, false);

            // Assert
            response.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Guess_DeckWithOneCard_ReturnException()
        {
            // Arrange
            var id = Guid.NewGuid();
            var deckJson = "[{\"Id\":\"226b70f2-0231-4457-9615-d00a56db8f84\",\"Value\":1,\"Suit\":0}]";
            var game = new Game()
            {
                Deck = [new Card(CardValue.Ace, CardSuit.Spades)],
                DeckJson = deckJson
            };
            var card = new Card(CardValue.Nine, CardSuit.Diamonds);
            _repositoryMock.Setup(x => x.GetGameById(id)).ReturnsAsync(game);

            // Act
            var ex = await Assert.ThrowsAsync<Exception>(async () => await _gameService.ProcessGuess(id, It.IsAny<bool>()));

            // Assert
            ex.Should().BeOfType<Exception>();
            ex.Message.Should().Be("No more cards in the deck to continue.");
        }

        [Fact]
        public async Task GetAllGames_With2ValidGames_ReturnOk()
        {
            // Arrange
            var deckJson = "[{\"Id\":\"226b70f2-0231-4457-9615-d00a56db8f84\",\"Value\":1,\"Suit\":0}, {\"Id\":\"f0b3f62e-d827-48a7-87b5-78590c9fe04b\",\"Value\":2,\"Suit\":1}]";
            var game1 = new Game()
            {
                Deck = [new Card(CardValue.Ace, CardSuit.Spades), new Card(CardValue.Nine, CardSuit.Diamonds), new Card(CardValue.Nine, CardSuit.Spades)],
                DeckJson = deckJson
            };
            var game2 = new Game()
            {
                Deck = [new Card(CardValue.Ace, CardSuit.Spades), new Card(CardValue.Nine, CardSuit.Diamonds), new Card(CardValue.Nine, CardSuit.Spades)],
                DeckJson = deckJson
            };
            List<Game> games = [game1, game2];
            _repositoryMock.Setup(x => x.GetAllGames()).ReturnsAsync(games);

            // Act
            var response = await _controller.GetAllGames();

            // Assert
            response.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)response).Value.Should().Be(games);
        }

        [Fact]
        public async Task GetScore_GameNotOver_ReturnsBadRequest()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var deckJson = "[{\"Id\":\"226b70f2-0231-4457-9615-d00a56db8f84\",\"Value\":1,\"Suit\":0}, {\"Id\":\"f0b3f62e-d827-48a7-87b5-78590c9fe04b\",\"Value\":2,\"Suit\":1}]";
            var game = new Game
            {
                Deck = [new Card(CardValue.Ace, CardSuit.Spades), new Card(CardValue.Nine, CardSuit.Diamonds), new Card(CardValue.Nine, CardSuit.Spades)],
                DeckJson = deckJson,
                Players = [new Player("Player 1"), new Player("Player 2")],
                CurrentPlayerIndex = 0
            };

            // Configura o repositório para retornar o jogo
            _repositoryMock.Setup(x => x.GetGameById(gameId)).ReturnsAsync(game);

            // Act
            var act = () => _controller.GetScore(gameId);

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("Game is not yet over.");
        }

        [Fact]
        public async Task GetScore_GameNotFound_ReturnsNotFound()
        {
            // Arrange
            var gameId = Guid.NewGuid();

            _repositoryMock.Setup(x => x.GetGameById(gameId)).ReturnsAsync((Game)null);

            // Act
            var act = () => _controller.GetScore(gameId);

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("Game not found.");
        }

        [Fact]
        public async Task GetScore_GameOver_ReturnsOkWithScore()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var deckJson = "[{\"Id\":\"226b70f2-0231-4457-9615-d00a56db8f84\",\"Value\":1,\"Suit\":0}, {\"Id\":\"f0b3f62e-d827-48a7-87b5-78590c9fe04b\",\"Value\":2,\"Suit\":1}]";
            var players = new List<Player>
            {
                new Player("Player 1") { Score = 10 },
                new Player("Player 2") { Score = 15 }
            };
            var game = new Game
            {
                Deck = [new Card(CardValue.Ace, CardSuit.Spades), new Card(CardValue.Nine, CardSuit.Diamonds), new Card(CardValue.Nine, CardSuit.Spades)],
                DeckJson = deckJson,
                Players = players,
                CurrentPlayerIndex = 2
            };

            _repositoryMock.Setup(x => x.GetGameById(gameId)).ReturnsAsync(game);

            // Act
            var response = await _controller.GetScore(gameId);

            // Assert
            response.Should().BeOfType<OkObjectResult>();
            var result = ((OkObjectResult)response).Value as List<Player>;
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(players);
        }

        private static GameDto GenerateGameDto()
        {
            var player1 = new PlayerDto();
            var player2 = new PlayerDto();
            List<PlayerDto> playerList = [player1, player2];
            var currentCard = new Card(CardValue.Ace, CardSuit.Spades);

            var gameDto = new GameDto() { GameId = Guid.NewGuid(), Players = playerList, CurrentCard = currentCard };
            return gameDto;
        }
    }
}
