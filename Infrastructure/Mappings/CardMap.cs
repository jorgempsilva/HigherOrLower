using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Mappings
{
    public static class CardMap
    {
        public static ModelBuilder Map(this ModelBuilder modelBuilder)
        {
            return modelBuilder.Entity<Card>(entity =>
            {
                entity.HasKey(c => c.Id);

                entity.Property(c => c.Suit).IsRequired();

                entity.Property(c => c.Value).IsRequired();

                entity.HasIndex(c => c.Suit);

                entity.HasIndex(c => c.Value);
            });
        }
    }
}
