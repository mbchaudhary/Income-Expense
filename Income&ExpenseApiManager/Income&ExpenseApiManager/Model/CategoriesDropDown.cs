using System.ComponentModel.DataAnnotations;

namespace Income_ExpenseApiManager.Model
{
    public class CategoriesDropDown
    {
        [Key]
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string CategoryType { get; set; }
    }
}
