using Microsoft.EntityFrameworkCore;
using PersonalFinance.Data;
using PersonalFinance.Models;
using PersonalFinance.Services.Interfaces;

namespace PersonalFinance.Services;

public class AssetTypeService : IAssetTypeService
{
    private readonly IDbContextFactory<FinanceDbContext> _dbContextFactory;

    public AssetTypeService(IDbContextFactory<FinanceDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<List<AssetType>> GetAssetTypesAsync()
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();

        return await context.AssetTypes
            .OrderBy(x => x.Name)
            .ToListAsync();
    }

    public async Task<AssetType?> GetAssetTypeByIdAsync(
        int id)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();

        return await context.AssetTypes
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<bool> AssetTypeExistsAsync(
        string name,
        int excludeId = 0)
    {
        name = name.Trim();

        await using var context = await _dbContextFactory.CreateDbContextAsync();

        return await context.AssetTypes.AnyAsync(
            x => x.Id != excludeId &&
                 x.Name.Trim().ToLower() ==
                 name.ToLower());
    }

    public async Task AddAssetTypeAsync(
        AssetType assetType)
    {
        assetType.Name = assetType.Name.Trim();

        await using var context = await _dbContextFactory.CreateDbContextAsync();

        context.AssetTypes.Add(assetType);

        await context.SaveChangesAsync();
    }

    public async Task UpdateAssetTypeAsync(
    AssetType assetType)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();

        var existingAssetType =
            await context.AssetTypes.FirstOrDefaultAsync(x => x.Id == assetType.Id);

        if (existingAssetType == null)
            return;

        existingAssetType.Name =
            assetType.Name.Trim();

        await context.SaveChangesAsync();
    }

    public async Task DeleteAssetTypeAsync(
        int id)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();

        var assetType =
            await context.AssetTypes.FirstOrDefaultAsync(x => x.Id == id);

        if (assetType == null)
            return;

        context.AssetTypes.Remove(assetType);

        await context.SaveChangesAsync();
    }
}