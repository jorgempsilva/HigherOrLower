namespace Domain.Entities
{
    public class Player(string name)
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public string Name { get; private set; } = name;
        public int Score { get; private set; } = 0;

        public void IncrementScore()
        {
            Score++;
        }
    }
}
