using PersonalFinance.Models;

namespace PersonalFinance.Data;

public static class DatabaseSeeder
{
    public static void Seed(FinanceDbContext context)
    {
        if (!context.Categories.Any())
        {
            context.Categories.AddRange(
                new Category { Name = "Food" },
                new Category { Name = "Transport" },
                new Category { Name = "Bills" },
                new Category { Name = "Shopping" },
                new Category { Name = "Medical" },
                new Category { Name = "Entertainment" },
                new Category { Name = "Education" },
                new Category { Name = "Travel" },
                new Category { Name = "Personal Care" },
                new Category
                {
                    Name = "Others",
                    IsDefault = true
                });
        }

        if (!context.AssetTypes.Any())
        {
            context.AssetTypes.AddRange(
                new AssetType { Name = "Mutual Fund" },
                new AssetType { Name = "Stock" },
                new AssetType { Name = "Fixed Deposit" },
                new AssetType { Name = "Gold" },
                new AssetType { Name = "Crypto" },
                new AssetType { Name = "Real Estate" },
                new AssetType { Name = "Other" });
        }

        context.SaveChanges();
    }
}