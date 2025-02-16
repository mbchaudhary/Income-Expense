using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Income_ExpenseApiManager.Model
{
    public class ExpenseModel
    {
        [Key]
        public int? ExpenseId { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal ExpenseAmount { get; set; }
        [Required]
        public string ExpenseCategory { get; set; }
        [Required]
        public DateTime ExpenseDate { get; set; }
        [MaxLength(255)]
        public string? Notes { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}
