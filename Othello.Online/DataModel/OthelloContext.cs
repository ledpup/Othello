using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Othello.Online.DataModel
{
    public class OthelloContext : DbContext
    {
        
        public DbSet<Game> Games { get; set; }
        public DbSet<User> Users { get; set; }

        public OthelloContext() : base()
        {
        }

        public OthelloContext(DbContextOptions<OthelloContext> dbContextOptions) : base(dbContextOptions)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                    .HasMany(u => u.GamesAsBlack)
                    .WithOne(tr => tr.UserBlack)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<User>()
                    .HasMany(u => u.GamesAsWhite)
                    .WithOne(tr => tr.UserWhite)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
