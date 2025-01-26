using System.ComponentModel.DataAnnotations;

namespace Income_ExpenseManager.Models
{
    public class CategoriesModel
    {
        [Key]

        public int CategoryId { get; set; }

        public string CategoryName { get; set; }

        public string CategoryType { get; set; }

        public int? UserId { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? ModifiedAt { get; set;}
    }
}
