using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Mappings
{
    public static class GameMap
    {
        public static ModelBuilder Map(this ModelBuilder modelBuilder)
        {
            return modelBuilder.Entity<Game>(entity =>
            {
                entity.HasKey(g => g.Id);

                entity.Property(g => g.CurrentCard).IsRequired();

                entity.HasMany(g => g.Players)
                    .WithOne(p => p.Game)
                    .HasForeignKey(x => x.GameId)
                    .IsRequired();
            });
        }
    }
}
