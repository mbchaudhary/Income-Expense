namespace Income_ExpenseApiManager.Model
{
    public class ReportModel
    {
        public decimal Amount{ get; set; }

        public String Category { get; set; }

        public DateTime Date { get; set; }

        public String? Notes { get; set; }

        public string Type { get; set; }
    }
}
