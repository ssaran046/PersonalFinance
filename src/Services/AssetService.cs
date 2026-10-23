using Microsoft.EntityFrameworkCore;
using PersonalFinance.Data;
using PersonalFinance.Models;
using PersonalFinance.Services.Interfaces;

namespace PersonalFinance.Services;

public class AssetService : IAssetService
{
    private readonly IDbContextFactory<FinanceDbContext> _dbContextFactory;

    public AssetService(IDbContextFactory<FinanceDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<List<Asset>> GetAssetsAsync()
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();

        return await context.Assets
            .Include(x => x.AssetType)
            .OrderBy(x => x.AssetType!.Name)
            .ThenBy(x => x.Name)
            .ToListAsync();
    }

    public async Task<Asset?> GetAssetByIdAsync(
        int id)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();

        return await context.Assets
            .Include(x => x.AssetType)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task AddAssetAsync(
        Asset asset)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();

        context.Assets.Add(asset);

        await context.SaveChangesAsync();
    }

    public async Task UpdateAssetAsync(
        Asset asset)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();

        var existingAsset = await context.Assets
            .FirstOrDefaultAsync(x => x.Id == asset.Id);

        if (existingAsset == null)
            return;

        existingAsset.Name = asset.Name;
        existingAsset.AssetTypeId = asset.AssetTypeId;
        existingAsset.Amount = asset.Amount;
        existingAsset.Notes = asset.Notes;

        await context.SaveChangesAsync();
    }

    public async Task DeleteAssetAsync(
        int id)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();

        var asset =
            await context.Assets.FirstOrDefaultAsync(x => x.Id == id);

        if (asset == null)
            return;

        context.Assets.Remove(asset);

        await context.SaveChangesAsync();
    }

    public async Task<decimal> GetTotalPortfolioAsync()
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();

        return (await context.Assets
            .Select(x => x.Amount)
            .ToListAsync())
            .Sum();
    }

    public async Task<List<AssetAllocationSummary>>
        GetAssetAllocationAsync()
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();

        var assets = await context.Assets
    .Include(x => x.AssetType)
    .ToListAsync();

        var total = assets.Sum(x => x.Amount);

        if (total == 0)
        {
            return [];
        }

        return assets
            .GroupBy(x => x.AssetType?.Name ?? "Unknown")
            .Select(g => new AssetAllocationSummary
            {
                AssetType = g.Key,
                Amount = g.Sum(x => x.Amount),
                Percentage =
                    (g.Sum(x => x.Amount) * 100m) / total
            })
            .OrderByDescending(x => x.Amount)
            .ToList();
    }
}