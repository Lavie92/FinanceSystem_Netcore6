﻿using FinanceSystem.Areas.Identity.Data;
using FinanceSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace FinanceSystem.Data
{
	public class FinanceSystemDbContext : IdentityDbContext
	{
		public FinanceSystemDbContext(DbContextOptions<FinanceSystemDbContext> options)
			: base(options)
		{
		}
		public DbSet<Category> Categories { get; set; }
		public DbSet<Transaction> Transactions { get; set; }
		public DbSet<Wallet> Wallets { get; set; }
        public DbSet<UserInfor> UserInfors { get; set; }
        public DbSet<PremiumSubscription> PremiumSubscriptions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<FinanceSystemUser>()
                .HasOne(u => u.UserInfor)
                .WithOne(p => p.User)
                .HasForeignKey<UserInfor>(p => p.Id);
			modelBuilder.Entity<FinanceSystemUser>()
				.HasOne(u => u.PremiumSubscription)
				.WithOne(p => p.User)
				.HasForeignKey<PremiumSubscription>(p => p.Id);
			//modelBuilder.Entity<Plan>()
   //    .Property(p => p.Amount)
   //    .HasConversion<double>();

   //         modelBuilder.Entity<Transaction>()
   //             .Property(t => t.Amount)
   //             .HasConversion<double>();

   //         modelBuilder.Entity<Wallet>()
   //             .Property(w => w.Balance)
   //             .HasConversion<double>();
        }
        public DbSet<FinanceSystem.Models.Plan>? Plan { get; set; }
        public DbSet<FinanceSystem.Models.TargetSaving>? TargetSaving { get; set; }
    }
}