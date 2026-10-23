namespace PersonalFinance.Models;

public class Expense
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int CategoryId { get; set; }

    public Category? Category { get; set; }

    public decimal Amount { get; set; }

    public DateTime Date { get; set; }

    public string? Notes { get; set; }
}