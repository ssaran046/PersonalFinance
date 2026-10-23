namespace PersonalFinance.Models;

public class AssetAllocationSummary
{
    public string AssetType { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public decimal Percentage { get; set; }
}