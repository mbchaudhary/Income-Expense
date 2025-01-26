using System.ComponentModel.DataAnnotations;

namespace Income_ExpenseApiManager.Model
{
    public class ExpenseModel
    {
        [Key]
        public int ExpenseId { get; set; }
        public int UserId { get; set; }
        public decimal ExpenseAmount { get; set; }
        public string ExpenseCategory { get; set; }
        public DateTime ExpenseDate { get; set; }
        public string Notes { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}
