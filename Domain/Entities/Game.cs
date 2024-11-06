using Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Game
    {
        public Guid Id { get; private set; }
        public int CurrentPlayerIndex { get; set; }
        [NotMapped]
        public List<Card> Deck { get; set; } = [];
        [NotMapped]
        public Card CurrentCard { get; set; }
        public List<Player> Players { get; set; } = [];
        public bool IsGameOver => CurrentPlayerIndex >= Deck.Count - 1;

        public Game()
        {
            if (Deck.Count == 0)
            {
                Deck = GenerateShuffledDeck();
                CurrentCard = Deck[0];
            }
            CurrentPlayerIndex = 0;
            Id = Guid.NewGuid();
        }

        public Player GetCurrentPlayer() => Players[CurrentPlayerIndex];

        public void NextPlayer()
        {
            CurrentPlayerIndex = (CurrentPlayerIndex + 1) % Players.Count;
        }

        private List<Card> GenerateShuffledDeck()
        {
            foreach (CardSuit suit in Enum.GetValues(typeof(CardSuit)))
            {
                foreach (CardValue value in Enum.GetValues(typeof(CardValue)))
                {
                    var card = new Card(value, suit);
                    Deck.Add(card);
                }
            }

            var random = new Random();
            for (var i = Deck.Count - 1; i > 0; i--)
            {
                var j = random.Next(i + 1);
                (Deck[i], Deck[j]) = (Deck[j], Deck[i]);
            }

            return Deck;
        }

        public void AddPlayer(string name)
        {
            var player = new Player(name)
            {
                GameId = this.Id,
                Game = this
            };
            Players.Add(player);
        }
    }
}
