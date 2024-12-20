﻿using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Mappings
{
    public static class PlayerMap
    {
        public static ModelBuilder Map(this ModelBuilder modelBuilder)
        {
            return modelBuilder.Entity<Player>(entity =>
            {
                entity.HasKey(p => p.Id);

                entity.Property(p => p.Name).IsRequired();

                entity.Property(p => p.Score).IsRequired();

                entity.Property(p => p.GameId).IsRequired();

                entity.Property(p => p.IsTurn).IsRequired();

                entity.HasIndex(p => p.Name);
                entity.HasIndex(p => p.GameId);
            });
        }
    }
}
