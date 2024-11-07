using Domain.Enums;
using System.Text.Json;

namespace Domain.Entities
{
    public class Game
    {
        public Guid Id { get; private set; }
        public int CurrentPlayerIndex { get; set; }
        public List<Card> Deck { get; set; } = [];
        public string DeckJson { get; set; }
        public Card CurrentCard { get; set; }
        public List<Player> Players { get; set; } = [];
        public bool IsGameOver => CurrentPlayerIndex >= Deck.Count - 1;

        public Game()
        {
            CurrentPlayerIndex = 0;
            Id = Guid.NewGuid();
        }

        public void InitializeDeck()
        {
            if (DeckJson == null)
            {
                Deck = GenerateShuffledDeck();
                CurrentCard = Deck[0];
                DeckJson = SerializeDeck(Deck);
            }
        }

        public void UpdateDeck(List<Card> deck)
        {
            CurrentCard = deck[0];
            DeckJson = SerializeDeck(deck);
        }

        private static string SerializeDeck(List<Card> deck) => JsonSerializer.Serialize(deck);

        private List<Card> DeserializeDeck(string deckJson) =>
            JsonSerializer.Deserialize<List<Card>>(deckJson) ?? [];

        public List<Card> LoadDeckFromJson() => Deck = DeserializeDeck(DeckJson);

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
