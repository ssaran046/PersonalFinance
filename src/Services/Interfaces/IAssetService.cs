using PersonalFinance.Models;

namespace PersonalFinance.Services.Interfaces;

public interface IAssetService
{
    Task<List<Asset>> GetAssetsAsync();

    Task<Asset?> GetAssetByIdAsync(int id);

    Task AddAssetAsync(Asset asset);

    Task UpdateAssetAsync(Asset asset);

    Task DeleteAssetAsync(int id);

    Task<decimal> GetTotalPortfolioAsync();

    Task<List<AssetAllocationSummary>>
        GetAssetAllocationAsync();
}