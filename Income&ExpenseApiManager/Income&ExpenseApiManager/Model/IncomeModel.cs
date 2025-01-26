using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Income_ExpenseApiManager.Model
{
    public class IncomeModel
    {
        [Key]
        public int IncomeID { get; set; }

        [Required]
        public int UserID { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal IncomeAmount { get; set; }

        [Required]
        public string IncomeSource { get; set; }

        [Required]
        public DateTime IncomeDate { get; set; }

        [MaxLength(255)]
        public string? Notes { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? ModifiedAt { get; set; }
    }
}
