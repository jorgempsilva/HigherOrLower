namespace Domain.Entities
{
    public class Player(string name)
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public string Name { get; private set; } = name;
        public int Score { get; private set; } = 0;
        public Guid GameId { get; set; }
        public Game Game { get; set; }
        public void IncrementScore()
        {
            Score++;
        }
    }
}
