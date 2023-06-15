using FinanceSystem.Areas.Identity.Data;
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<FinanceSystemUser>()
                .HasOne(u => u.UserInfor)
                .WithOne(p => p.User)
                .HasForeignKey<UserInfor>(p => p.Id)
                .OnDelete(DeleteBehavior.Cascade);
            //modelBuilder.Entity<Plan>()
            //    .Property(p => p.Amount)
            //    .HasConversion<double>();

            //         modelBuilder.Entity<Transaction>()
            //             .Property(t => t.Amount)
            //             .HasConversion<double>();

            //         modelBuilder.Entity<Wallet>()
            //             .Property(w => w.Balance)
            //             .HasConversion<double>();
            modelBuilder.Entity<FinanceSystemUser>()
                 .HasMany(u => u.Wallets)
                 .WithOne()
                 .HasForeignKey(w => w.UserId)
                 .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<FinanceSystemUser>()
                 .HasMany(u => u.Categorys)
                 .WithOne()
                 .HasForeignKey(w => w.UserId)
                 .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<FinanceSystemUser>()
                 .HasMany(u => u.Plans)
                 .WithOne()
                 .HasForeignKey(w => w.UserId)
                 .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<FinanceSystemUser>()
                 .HasMany(u => u.TargetSavings)
                 .WithOne()
                 .HasForeignKey(w => w.UserId)
                 .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Category)
                .WithMany(c => c.Transactions)
                .HasForeignKey(t => t.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Wallet)
                .WithMany(w => w.Transactions)
                .HasForeignKey(t => t.WalletId)
                .OnDelete(DeleteBehavior.Cascade);

        }
        public DbSet<FinanceSystem.Models.Plan>? Plan { get; set; }
        public DbSet<FinanceSystem.Models.TargetSaving>? TargetSaving { get; set; }
    }
}