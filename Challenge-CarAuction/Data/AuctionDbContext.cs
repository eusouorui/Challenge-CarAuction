﻿using ChallengeCarAuction.Models;
using Microsoft.EntityFrameworkCore;

namespace ChallengeCarAuction
{
    public class AuctionDbContext : DbContext
    {
        public AuctionDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions) 
        {
            
        }

        #region DbSets

        public DbSet<Auction> Auctions { get; set; }
        public DbSet<Bid> Bids { get; set; }
        public DbSet<Manufacturer> Manufacturers { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<Model> Models { get; set; }

        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // One-to-Many relationship between Manufacturer and Models
            modelBuilder.Entity<Model>()
                .HasOne(m => m.Manufacturer)
                .WithMany(mf => mf.Models)
                .HasForeignKey(m => m.ManufacturerId);

            // One-to-Many relationship between Model and Cars
            modelBuilder.Entity<Car>()
                .HasOne(c => c.Model)
                .WithMany(m => m.Cars)
                .HasForeignKey(c => c.ModelId);

        }
    }
}