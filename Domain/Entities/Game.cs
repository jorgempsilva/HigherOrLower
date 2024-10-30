namespace Domain.Entities
{
    public class Game
    {
        private readonly List<string> _deck;
        private int _currentCardIndex;
        public Guid Id { get; private set; }
        public string CurrentCard { get; private set; }
        public List<Player> Players { get; private set; } = [];
        public bool IsGameOver => _currentCardIndex >= _deck.Count - 1;

        public Game()
        {
            _deck = GenerateShuffledDeck();
            _currentCardIndex = 0;
            Id = Guid.NewGuid();
            CurrentCard = _deck[_currentCardIndex];
        }

        public bool MakeGuess(Guid gameId, Guid playerId, bool guessHigher)
        {
            if (IsGameOver)
                throw new InvalidOperationException("The game is already over.");

            var previousCard = CurrentCard;
            _currentCardIndex++;
            CurrentCard = _deck[_currentCardIndex];

            var isCorrect = CompareCards(previousCard, CurrentCard, guessHigher);
            if (isCorrect)
            {
                var player = Players.Where(p => p.Id == playerId).FirstOrDefault() ?? throw new InvalidOperationException("The player doesn't exist.");
                player.IncrementScore();
            }

            return isCorrect;
        }

        private static bool CompareCards(string previousCard, string nextCard, bool guessHigher)
        {
            var prevValue = GetCardValue(previousCard);
            var nextValue = GetCardValue(nextCard);

            return guessHigher ? nextValue >= prevValue : nextValue <= prevValue;
        }

        private static int GetCardValue(string card)
        {
            var value = card[..^1];
            return value switch
            {
                "A" => 1,
                "J" => 11,
                "Q" => 12,
                "K" => 13,
                _ => Convert.ToInt32(value)
            };
        }

        private static List<string> GenerateShuffledDeck()
        {
            var deck = new List<string>
            {
                "AS", "2S", "3S", "4S", "5S", "6S", "7S", "8S", "9S", "10S", "JS", "QS", "KS",
                "AH", "2H", "3H", "4H", "5H", "6H", "7H", "8H", "9H", "10H", "JH", "QH", "KH",
                "AD", "2D", "3D", "4D", "5D", "6D", "7D", "8D", "9D", "10D", "JD", "QD", "KD",
                "AC", "2C", "3C", "4C", "5C", "6C", "7C", "8C", "9C", "10C", "JC", "QC", "KC"
            };

            var random = new Random();
            for (var i = deck.Count - 1; i > 0; i--)
            {
                var j = random.Next(i + 1);
                (deck[i], deck[j]) = (deck[j], deck[i]);
            }

            return deck;
        }

        public void AddPlayer(string name)
        {
            Players.Add(new Player(name));
        }
    }
}
