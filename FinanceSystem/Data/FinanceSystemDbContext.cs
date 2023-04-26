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
            Categories = Set<Category>();
            Transactions = Set<Transaction>();
            Wallets = Set<Wallet>();
            UserInfors = Set<UserInfor>();
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
                .HasForeignKey<UserInfor>(p => p.Id);
        }
    }
}