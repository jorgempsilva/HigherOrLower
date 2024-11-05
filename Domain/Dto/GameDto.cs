namespace Domain.Dto
{
    public class GameDto
    {
        public Guid Id { get; set; }
        public string CurrentCard { get; set; }
        public List<PlayerDto> Players { get; set; } = [];
    }
}
