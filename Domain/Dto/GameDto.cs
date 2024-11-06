using Domain.Entities;

namespace Domain.Dto
{
    public class GameDto
    {
        public Guid GameId { get; set; }
        public Card CurrentCard { get; set; }
        public List<PlayerDto> Players { get; set; } = [];
    }
}
