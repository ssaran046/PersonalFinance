using System.ComponentModel.DataAnnotations;

namespace PersonalFinance.Models;

public class Category
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    public bool IsDefault { get; set; }
}