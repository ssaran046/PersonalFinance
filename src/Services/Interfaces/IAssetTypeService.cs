using PersonalFinance.Models;

namespace PersonalFinance.Services.Interfaces;

public interface IAssetTypeService
{
    Task<List<AssetType>> GetAssetTypesAsync();

    Task<AssetType?> GetAssetTypeByIdAsync(int id);

    Task<bool> AssetTypeExistsAsync(
        string name,
        int excludeId = 0);

    Task AddAssetTypeAsync(AssetType assetType);

    Task UpdateAssetTypeAsync(AssetType assetType);

    Task DeleteAssetTypeAsync(int id);
}