namespace Income_ExpenseManager.Models
{
    public class TransactionModel
    {
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public decimal Amount { get; set; }
    }
}
