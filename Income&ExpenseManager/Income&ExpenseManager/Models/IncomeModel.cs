using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Income_ExpenseManager.Models
{
    public class IncomeModel
    {
        [Key]
        public int? IncomeID { get; set; }

        [Required]
        public int UserID { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal IncomeAmount { get; set; }

        [Required]
        [MaxLength(50)]
        public string IncomeSource { get; set; }


        [Required]
        public DateTime IncomeDate { get; set; }

        [MaxLength(255)]
        public string? Notes { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? ModifiedAt { get; set; }
    }


    public class setCategoriesDropDown
    {
        public int CategoryId { get; set; }

        public string CategoryName { get; set; }
    }

    public class DateNotInPastAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is DateTime dateTime)
            {
                if (dateTime.Date < DateTime.Now.Date)
                {
                    return new ValidationResult(ErrorMessage ?? "Date cannot be in the past.");
                }
            }
            return ValidationResult.Success;
        }
    }
}
