using System.ComponentModel.DataAnnotations;

namespace Income_ExpenseManager.Models
{
    public class CategoriesModel
    {
        [Key]

        public int? CategoryId { get; set; }
        [Required]
        public string CategoryName { get; set; }
        [Required]
        public string CategoryType { get; set; }
        [Required]
        public int? UserId { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? ModifiedAt { get; set;}
    }
}
