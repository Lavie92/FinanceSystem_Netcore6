using FinanceSystem.Data;
using Microsoft.EntityFrameworkCore;

namespace FinanceSystem.Models
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new FinanceSystemDbContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<FinanceSystemDbContext>>()))
            {
                // Look for any movies.
                if (context.Wallets.Any())
                {
                    return;   // DB has been seeded
                }

                context.Wallets.AddRange(
                    new Wallet
                    {
                        Id = 1,
                        UserId = "1",
                        Balance = 2222,
                        Name = "An",
                    },

                    new Wallet
                    {
                        Id = 2,
                        UserId = "4",
                        Balance = 5555,
                        Name = "A1n",
                    },

                    new Wallet
                    {
                        Id = 3,
                        UserId = "55",
                        Balance = 2222,
                        Name = "An5",
                    },

                    new Wallet
                    {
                        Id = 4,
                        UserId = "6",
                        Balance = 787,
                        Name = "12wqd",
                    }
                );
                context.SaveChanges();
            }
        }
    }
}
