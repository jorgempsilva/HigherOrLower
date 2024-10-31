using Domain.Entities;
using Infrastructure.Mappings;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Contexts
{
    public class SqlContext(DbContextOptions<SqlContext> options) : DbContext(options)
    {
        public DbSet<Game> Games { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Card> Cards { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Game>().Ignore(g => g.IsGameOver);
            base.OnModelCreating(modelBuilder);

            CardMap.Map(modelBuilder);
            PlayerMap.Map(modelBuilder);
            GameMap.Map(modelBuilder);
        }
    }
}
