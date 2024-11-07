namespace Domain.Entities
{
    public class Player
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public string Name { get; private set; }
        public int Score { get; set; } = 0;
        public Guid GameId { get; set; }
        public bool IsTurn { get; set; }
        public virtual Game Game { get; set; }

        public Player(string name)
        {
            Name = name;
        }

        public Player() { }
    }
}
