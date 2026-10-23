using System.ComponentModel.DataAnnotations;

namespace PersonalFinance.Models;

public class AssetType
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;
}