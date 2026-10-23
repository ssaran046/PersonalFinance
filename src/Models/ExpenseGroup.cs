using System.Collections.ObjectModel;

namespace PersonalFinance.Models;

public class ExpenseGroup : ObservableCollection<Expense>
{
    public ExpenseGroup(DateTime date, IEnumerable<Expense> expenses)
        : base(expenses)
    {
        Date = date;
        TotalAmount = expenses.Sum(x => x.Amount);
    }

    public DateTime Date { get; }

    public decimal TotalAmount { get; }

    public string Header =>
        Date.Date == DateTime.Today
            ? "Today"
            : Date.Date == DateTime.Today.AddDays(-1)
                ? "Yesterday"
                : Date.ToString("dd MMM yyyy");
}