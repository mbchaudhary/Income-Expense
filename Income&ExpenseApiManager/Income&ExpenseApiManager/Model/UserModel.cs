using System.ComponentModel.DataAnnotations;

namespace Income_ExpenseApiManager.Model
{
    public class UserModel
    {
        [Key]

        public int UserID {  get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public DateTime? CeratedAt { get; set; }

        public DateTime? ModifiedAt { get; set; }
    }
}
