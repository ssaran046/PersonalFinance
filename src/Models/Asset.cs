namespace PersonalFinance.Models;

public class Asset
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int AssetTypeId { get; set; }

    public decimal Amount { get; set; }

    public AssetType? AssetType { get; set; }

    public string? Notes { get; set; }
}